using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using DMCopilot.Backend.Controllers;
using DMCopilot.Backend.Extensions;
using DMCopilot.Data.Repositories;
using DMCopilot.Services;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;


namespace DMCopilot.Backend;

public sealed class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // builder.Host.AddConfiguration();

        // Load configuration
        builder.Services.AddOptions(builder.Configuration);

        // Add Azure Credential service
        builder.Services.AddAzureCredentialService();

        // Add logging and application insights service
        builder.Services.AddLoggingAndTelemetry(builder.Configuration);

        // Add authentication related services
        builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

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

        // Add support for controllers and identity pages
        builder.Services
            .AddControllersWithViews()
            .AddMicrosoftIdentityUI();

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor()
            .AddMicrosoftIdentityConsentHandler();

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

        // Add health checks
        builder.Services.AddHealthChecks();

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
        app.MapHealthChecks("/healthz");
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}