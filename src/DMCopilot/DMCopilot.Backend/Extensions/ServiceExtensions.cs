using DMCopilot.Data.Repositories;
using DMCopilot.Entities.Models;
using DMCopilot.Services;
using DMCopilot.Services.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace DMCopilot.Backend.Extensions;

public static class BackendServiceExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Add the Azure AD Configuration options
        services.AddOptions<AuthorizationOptions>(AuthorizationOptions.PropertyName)
            .Bind(configuration.GetSection(AuthorizationOptions.PropertyName))
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        // Add the Microsoft Graph Configuration options
        services.AddOptions<MicrosoftGraphOptions>(MicrosoftGraphOptions.PropertyName)
            .Bind(configuration.GetSection(MicrosoftGraphOptions.PropertyName))
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        // Add the Azure App Configuration options
        services.AddOptions<AppConfigurationOptions>(AppConfigurationOptions.PropertyName)
            .Bind(configuration.GetSection(AppConfigurationOptions.PropertyName))
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        // Add the Data Store options
        services.AddOptions<DataStoreOptions>(DataStoreOptions.PropertyName)
            .Bind(configuration.GetSection(DataStoreOptions.PropertyName))
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        return services;
    }

    /// <summary>
    /// Add authorization services
    /// </summary>
    internal static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Complete this method

        var authorizationOptions = services.BuildServiceProvider().GetRequiredService<IOptions<AuthorizationOptions>>().Value;
        switch (authorizationOptions.Type)
        {
            case AuthorizationOptions.AuthorizationType.AzureAd:
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(configuration.GetSection($"{AuthorizationOptions.PropertyName}:AzureAd"));
                break;

            case AuthorizationOptions.AuthorizationType.None:
                services.AddAuthentication(PassThroughAuthenticationHandler.AuthenticationScheme)
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
    internal static IServiceCollection AddAzureCredentialService(this IServiceCollection services)
    {
        services.AddSingleton<AzureCredentialService>((service) =>
        {
            return new AzureCredentialService(service.GetService<IOptions<AuthorizationOptions>>());
        });

        return services;
    }

    /// <summary>
    /// Add Semantic Kernel service
    /// </summary>
    internal static IServiceCollection AddSemanticKernel(this IServiceCollection services)
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
        IStorageContext<Character> characterStorageContext;

        var dataStoreConfig = services.BuildServiceProvider().GetRequiredService<IOptions<DataStoreOptions>>().Value;

        switch (dataStoreConfig.Type)
        {
            case DataStoreOptions.DataStoreType.CosmosDb:
                {
                    if (dataStoreConfig.CosmosDb == null)
                    {
                        throw new InvalidOperationException("DataStore:Cosmos is required when DataStore:Type is 'CosmosDb'");
                    }
#pragma warning disable CA2000 // Dispose objects before losing scope - objects are singletons for the duration of the process and disposed when the process exits.
                    accountStorageContext = new CosmosDbContext<Account>(
                        dataStoreConfig.CosmosDb.ConnectionString, dataStoreConfig.CosmosDb.DatabaseName, dataStoreConfig.CosmosDb.AccountsContainerName);
                    tenantStorageContext = new CosmosDbContext<Tenant>(
                        dataStoreConfig.CosmosDb.ConnectionString, dataStoreConfig.CosmosDb.DatabaseName, dataStoreConfig.CosmosDb.TenantsContainerName);
                    worldStorageContext = new CosmosDbContext<World>(
                        dataStoreConfig.CosmosDb.ConnectionString, dataStoreConfig.CosmosDb.DatabaseName, dataStoreConfig.CosmosDb.WorldsContainerName);
                    characterStorageContext = new CosmosDbContext<Character>(
                        dataStoreConfig.CosmosDb.ConnectionString, dataStoreConfig.CosmosDb.DatabaseName, dataStoreConfig.CosmosDb.CharactersContainerName);
#pragma warning restore CA2000 // Dispose objects before losing scope
                    break;
                }

            default:
                {
                    throw new InvalidOperationException(
                        "Invalid 'ChatStore' setting 'chatStoreConfig.Type'.");
                }
        }

        services.AddSingleton<AccountRepository>(new AccountRepository(accountStorageContext));
        services.AddSingleton<TenantRepository>(new TenantRepository(tenantStorageContext));
        services.AddSingleton<WorldRepository>(new WorldRepository(worldStorageContext));
        services.AddSingleton<CharacterRepository>(new CharacterRepository(characterStorageContext));

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