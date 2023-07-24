using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Cosmos;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using DMCopilot.Data.Repositories;
using DMCopilot.Backend.Controllers;
using DMCopilot.Shared.Services;
using DMCopilot.Shared.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

        // Add services to the container.
        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
        .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
                    .AddInMemoryTokenCaches();
        builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();

        // Get an Azure AD token for the application to use to authenticate to services in Azure
        var azureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            TenantId = builder.Configuration["AzureAd:TenantId"]
        });

        // Add the Application Configuration using the endpoint and set the options to connect
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var appConfigurationEndpoint = builder.Configuration["AppConfiguration:Endpoint"] ?? throw new Exception("Semantic Kernel configuration is null");

            UriBuilder appConfigurationEndpointUriBuilder = new UriBuilder(appConfigurationEndpoint);
            options.Connect(appConfigurationEndpointUriBuilder.Uri, azureCredential);
        }
        );

        // Add Application Insights services into service collection
        builder.Services.AddApplicationInsightsTelemetry();

        // Add Logging services into service collection
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddApplicationInsights();
        });

        builder.Services.AddAuthorization(options =>
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

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor()
            .AddMicrosoftIdentityConsentHandler();

        // Add the Cosmos DB client as a singleton service
        var cosmosDbConfiguration = builder.Configuration.GetSection("CosmosDb").Get<CosmosDbConfiguration>() ?? throw new Exception("CosmosDb configuration is null");

        builder.Services.AddSingleton<CosmosClient>(service =>
            {
                var cosmosDbConnectionString = builder.Configuration.GetConnectionString("CosmosDb");
                if (string.IsNullOrEmpty(cosmosDbConnectionString))
                {
                    var cosmosDbEndpoint = cosmosDbConfiguration.EndpointUri ?? throw new Exception("CosmosDb:EndpointUri is null");
                    return new CosmosClient(cosmosDbEndpoint.ToString(), azureCredential);
                }
                else
                {
                    return new CosmosClient(cosmosDbConnectionString);
                }
            });

        // Define an array of repository configurations
        var repositories = new[] {
            new { RepositoryType = typeof(AccountRepositoryCosmosDb), RepositoryInterface = typeof(IAccountRepository), CollectionName = "accounts" },
            new { RepositoryType = typeof(TenantRepositoryCosmosDb), RepositoryInterface = typeof(ITenantRepository), CollectionName = "tenants" },
            new { RepositoryType = typeof(WorldRepositoryCosmosDb), RepositoryInterface = typeof(IWorldRepository), CollectionName = "worlds" },
            new { RepositoryType = typeof(CharacterRepositoryCosmosDb), RepositoryInterface = typeof(ICharacterRepository), CollectionName = "characters" }
        };

        // Loop through the repository configurations and register them as scoped services
        foreach (var repository in repositories)
        {
            builder.Services.AddScoped(repository.RepositoryInterface, service =>
            {
                var cosmosClient = service.GetService<CosmosClient>();
                var loggerType = typeof(ILogger<>).MakeGenericType(repository.RepositoryType);
                var logger = service.GetService(loggerType);
                var repositoryInstance = Activator.CreateInstance(repository.RepositoryType, cosmosClient, cosmosDbConfiguration.DatabaseName, repository.CollectionName, logger);
                return repositoryInstance;
            });
        }

        // Add the Account Service
        builder.Services.AddScoped<IAccessService>((service) =>
        {
            var accountRepository = service.GetService<IAccountRepository>();
            var tenantRepository = service.GetService<ITenantRepository>();
            return new AccessService(accountRepository, tenantRepository, service.GetService<ILogger<AccessService>>());
        });

        // Add the Semantic Kernel service
        builder.Services.AddSingleton<ISemanticKernelService>((service) =>
        {
            // Get the Semantic Kernel configuration from appsettings.json
            var semanticKernelConfiguration = builder.Configuration
                .GetSection("SemanticKernel")
                .Get<SemanticKernelConfiguration>() ?? throw new Exception("Semantic Kernel configuration is null");
            if (semanticKernelConfiguration.AzureOpenAiApiKey == null)
                return new SemanticKernelService(azureCredential, semanticKernelConfiguration, service.GetService<ILogger<SemanticKernelService>>());
            else
                return new SemanticKernelService(semanticKernelConfiguration, service.GetService<ILogger<SemanticKernelService>>());
        });

        // Add Blazorize
        builder.Services.AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

        // Add the controllers and the Health Check
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HealthCheckController).Assembly)
            .AddControllersAsServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}