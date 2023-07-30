using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using DMCopilot.Backend.Controllers;
using DMCopilot.Backend.Extensions;
using DMCopilot.Data.Repositories;
using DMCopilot.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Load configuration
        builder.Services.AddOptions(builder.Configuration);

        // Add logging and application insights service
        builder.Services
            .AddSingleton<ILogger>(sp => sp.GetRequiredService<ILogger<Program>>()) // some services require an un-templated ILogger
            .AddSingleton<ILoggerFactory, LoggerFactory>()
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddApplicationInsights();
            })
            .AddApplicationInsightsTelemetry();

        // TODO: Add Application Configuration service

        // TODO: Move this to Authorization service

        // Add authentication related services
        var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');
        builder.Services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
            .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();

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

        // Add Azure Credential service
        builder.Services.AddAzureCredentialService();

        // Add support for controllers and identity pages
        builder.Services
            .AddControllersWithViews()
            .AddMicrosoftIdentityUI();

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor()
            .AddMicrosoftIdentityConsentHandler();

        // Add the Data Store
        builder.Services.AddDataStore();

        // Add the Account Service
        builder.Services.AddScoped<IAccessService>((service) =>
        {
            var accountRepository = service.GetService<AccountRepository>();
            var tenantRepository = service.GetService<TenantRepository>();
            return new AccessService(service.GetService<ILogger<AccessService>>(), accountRepository, tenantRepository);
        });

        // Add the Semantic Kernel service
        builder.Services.AddSemanticKernel();

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