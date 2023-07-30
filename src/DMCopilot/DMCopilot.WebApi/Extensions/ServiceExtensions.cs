using DMCopilot.WebApi.Extensions;
using DMCopilot.Shared.Services.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Reflection;

namespace DMCopilot.WebApi.Extensions;

internal static class ServicesExtensions
{
    /// <summary>
    /// Parse configuration into options.
    /// </summary>
    internal static IServiceCollection AddOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        // AzureAdOptions configuration
        services.AddOptions<AuthorizationOptions>()
            .Bind(configuration.GetSection(AuthorizationOptions.PropertyName))
            .ValidateOnStart()
            .ValidateDataAnnotations()
            .PostConfigure(TrimStringProperties);

        // Microsoft Graph configuration
        services.AddOptions<MicrosoftGraphOptions>()
            .Bind(configuration.GetSection(MicrosoftGraphOptions.PropertyName))
            .ValidateOnStart()
            .ValidateDataAnnotations()
            .PostConfigure(TrimStringProperties);

        // App Configuration configuration
        services.AddOptions<AppConfigurationOptions>()
            .Bind(configuration.GetSection(AppConfigurationOptions.PropertyName))
            .ValidateDataAnnotations()
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
    /// Add CORS settings.
    /// </summary>
    internal static IServiceCollection AddCors(this IServiceCollection services)
    {
        IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        string[] allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        if (allowedOrigins.Length > 0)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                            .AllowAnyHeader();
                    });
            });
        }

        return services;
    }

    /// <summary>
    /// Add authorization services
    /// </summary>
    internal static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        Microsoft.AspNetCore.Authorization.AuthorizationOptions config = services.BuildServiceProvider().GetRequiredService<IOptions<Microsoft.AspNetCore.Authorization.AuthorizationOptions>>().Value;
        switch (config.Type)
        {
            case Microsoft.AspNetCore.Authorization.AuthorizationOptions.AuthorizationType.AzureAd:
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(configuration.GetSection($"{Microsoft.AspNetCore.Authorization.AuthorizationOptions.PropertyName}:AzureAd"));
                break;

            case Microsoft.AspNetCore.Authorization.AuthorizationOptions.AuthorizationType.ApiKey:
                services.AddAuthentication(ApiKeyAuthenticationHandler.AuthenticationScheme)
                    .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                        ApiKeyAuthenticationHandler.AuthenticationScheme,
                        options => options.ApiKey = config.ApiKey);
                break;

            case Microsoft.AspNetCore.Authorization.AuthorizationOptions.AuthorizationType.None:
                services.AddAuthentication(PassThroughAuthenticationHandler.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, PassThroughAuthenticationHandler>(
                        authenticationScheme: PassThroughAuthenticationHandler.AuthenticationScheme,
                        configureOptions: null);
                break;

            default:
                throw new InvalidOperationException($"Invalid authorization type '{config.Type}'.");
        }

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