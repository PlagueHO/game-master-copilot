using DMCopilot.Data.Repositories;
using DMCopilot.Entities.Models;
using DMCopilot.Services;
using DMCopilot.Services.Options;
using DMCopilot.Services.Auth;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

using System.Reflection;

namespace DMCopilot.Backend.Extensions;

public static class BackendServiceExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Add the Azure Application Insights options
        services.AddOptions<ApplicationInsightsOptions>()
            .Bind(configuration.GetSection(ApplicationInsightsOptions.PropertyName))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        // Add the Azure AD Configuration options
        services.AddOptions<Services.Options.AuthorizationOptions>()
            .Bind(configuration.GetSection(Services.Options.AuthorizationOptions.PropertyName))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        // Add the Microsoft Graph Configuration options
        services.AddOptions<Services.Options.MicrosoftGraphOptions>()
            .Bind(configuration.GetSection(Services.Options.MicrosoftGraphOptions.PropertyName))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        // Add the Azure App Configuration options
        services.AddOptions<AppConfigurationOptions>()
            .Bind(configuration.GetSection(AppConfigurationOptions.PropertyName))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        // Add the Data Store options
        services.AddOptions<DataStoreOptions>()
            .Bind(configuration.GetSection(DataStoreOptions.PropertyName))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        return services;
    }

    /// <summary>
    /// Add logging and telemetry services
    /// </summary>
    public static IServiceCollection AddLoggingAndTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationInsightsConnectionString = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationInsightsOptions>>().Value.ConnectionString;

        services
            .AddSingleton<ILogger>(sp => sp.GetRequiredService<ILogger<Program>>()) // some services require an un-templated ILogger
            .AddSingleton<ILoggerFactory, LoggerFactory>()
            .AddHttpContextAccessor()
            .AddApplicationInsightsTelemetry(options => options.ConnectionString = applicationInsightsConnectionString)
            .AddSingleton<ITelemetryInitializer, AppInsightsUserTelemetryInitializerService>()
            .AddLogging(logBuilder =>
            {
                logBuilder.AddConsole();
                logBuilder.AddApplicationInsights();
            })
            .AddSingleton<ITelemetryService, AppInsightsTelemetryService>();

        return services;
    }

    /// <summary>
    /// Add authentication and authorization services
    /// </summary>
    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var authorizationOptions = services.BuildServiceProvider().GetRequiredService<IOptions<Services.Options.AuthorizationOptions>>().Value;

        switch (authorizationOptions.Type)
        {
            case Services.Options.AuthorizationOptions.AuthorizationType.AzureAd:
                var initialScopes = configuration["DownstreamApi:Scopes"]?.Split(' ') ?? configuration["MicrosoftGraph:Scopes"]?.Split(' ');
                services
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApp(configuration.GetSection($"{Services.Options.AuthorizationOptions.PropertyName}:AzureAd"))
                    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                    .AddMicrosoftGraph(configuration.GetSection("MicrosoftGraph"))
                    .AddInMemoryTokenCaches();

                services.AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.AddPolicy("AllowAnonymous", policy =>
                    {
                        policy.RequireAssertion(context =>
                        {
                            // Allow unauthenticated access to the HealthCheck endpoint
                            return context.Resource is Endpoint endpoint &&
                                   endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null;
                        });
                    });

                    // By default, all incoming requests will be authorized according to the default policy
                    options.FallbackPolicy = options.DefaultPolicy;
                });

                break;

            case Services.Options.AuthorizationOptions.AuthorizationType.None:
                services
                    .AddAuthentication(PassThroughAuthenticationHandler.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, PassThroughAuthenticationHandler>(
                        authenticationScheme: PassThroughAuthenticationHandler.AuthenticationScheme,
                        configureOptions: null);
                break;

            default:
                throw new InvalidOperationException($"Invalid authorization type '{authorizationOptions.Type}'.");
        }

        return services;
    }

    /// <summary>
    /// Add Azure Credential Service
    /// </summary>
    public static IServiceCollection AddAzureCredentialService(this IServiceCollection services)
    {
        services.AddSingleton<AzureCredentialService>((service) =>
        {
            return new AzureCredentialService(service.GetService<IOptions<Services.Options.AuthorizationOptions>>());
        });

        return services;
    }

    /// <summary>
    /// Add Semantic Kernel service
    /// </summary>
    public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
    {
        services.AddSingleton<ISemanticKernelService>((service) =>
        {
            var semanticKernelOptions = service.GetService<IOptions<SemanticKernelOptions>>().Value;
            if (semanticKernelOptions.AzureOpenAiApiKey == null)
            {
                var azureDefaultCredential = service.GetService<AzureCredentialService>().GetDefaultAzureCredential();
                return new SemanticKernelService(service.GetService<ILogger<SemanticKernelService>>(), azureDefaultCredential, service.GetService<IOptions<SemanticKernelOptions>>());
            }
            else
                return new SemanticKernelService(service.GetService<ILogger<SemanticKernelService>>(), service.GetService<IOptions<SemanticKernelOptions>>());
        });

        return services;
    }

    /// <summary>
    /// Add data store services.
    /// </summary>
    public static IServiceCollection AddDataStore(this IServiceCollection services)
    {
        IStorageContext<Account> accountStorageContext;
        IStorageContext<Tenant> tenantStorageContext;
        IStorageContext<World> worldStorageContext;

        var dataStoreConfig = services.BuildServiceProvider().GetRequiredService<IOptions<DataStoreOptions>>().Value;

        switch (dataStoreConfig.Type)
        {
            case DataStoreOptions.DataStoreType.CosmosDb:
                {
                    if (dataStoreConfig.CosmosDb == null)
                    {
                        throw new InvalidOperationException("DataStore:CosmosDb is required when DataStore:Type is 'CosmosDb'");
                    }
#pragma warning disable CA2000 // Dispose objects before losing scope - objects are singletons for the duration of the process and disposed when the process exits.
                    accountStorageContext = new CosmosDbContext<Account>(
                        dataStoreConfig.CosmosDb.ConnectionString, dataStoreConfig.CosmosDb.DatabaseName, dataStoreConfig.CosmosDb.AccountsContainerName);
                    tenantStorageContext = new CosmosDbContext<Tenant>(
                        dataStoreConfig.CosmosDb.ConnectionString, dataStoreConfig.CosmosDb.DatabaseName, dataStoreConfig.CosmosDb.TenantsContainerName);
                    worldStorageContext = new CosmosDbContext<World>(
                        dataStoreConfig.CosmosDb.ConnectionString, dataStoreConfig.CosmosDb.DatabaseName, dataStoreConfig.CosmosDb.WorldsContainerName);
#pragma warning restore CA2000 // Dispose objects before losing scope
                    break;
                }

            default:
                {
                    throw new InvalidOperationException(
                        $"Invalid 'DataStore:Type' setting '{dataStoreConfig.Type}'.");
                }
        }

        services.AddSingleton<AccountRepository>(new AccountRepository(accountStorageContext));
        services.AddSingleton<TenantRepository>(new TenantRepository(tenantStorageContext));
        services.AddSingleton<WorldRepository>(new WorldRepository(worldStorageContext));

        return services;
    }

    /// <summary>
    /// Trim all string properties, recursively.
    /// </summary>
    private static void TrimStringProperties<T>(T options) where T : class
    {
        Queue<object> targets = new();
        targets.Enqueue(options);

        while (targets.Count > 0)
        {
            object target = targets.Dequeue();
            Type targetType = target.GetType();
            foreach (PropertyInfo property in targetType.GetProperties())
            {
                // Skip enumerations
                if (property.PropertyType.IsEnum)
                {
                    continue;
                }

                // Property is a built-in type, readable, and writable.
                if (property.PropertyType.Namespace == "System" &&
                    property.CanRead &&
                    property.CanWrite)
                {
                    // Property is a non-null string.
                    if (property.PropertyType == typeof(string) &&
                        property.GetValue(target) != null)
                    {
                        property.SetValue(target, property.GetValue(target)!.ToString()!.Trim());
                    }
                }
                else
                {
                    // Property is a non-built-in and non-enum type - queue it for processing.
                    if (property.GetValue(target) != null)
                    {
                        targets.Enqueue(property.GetValue(target)!);
                    }
                }
            }
        }
    }
}