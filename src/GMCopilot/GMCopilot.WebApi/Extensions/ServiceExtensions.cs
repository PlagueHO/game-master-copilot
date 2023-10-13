using GMCopilot.Data.Repositories;
using GMCopilot.Entities.Models;
using GMCopilot.Services;
using GMCopilot.Services.Options;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace GMCopilot.WebApi.Extensions;

public static class BackendServiceExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Add the Azure Application Insights options
        services.AddOptions<ApplicationInsightsOptions>(ApplicationInsightsOptions.PropertyName)
            .Bind(configuration.GetSection(ApplicationInsightsOptions.PropertyName))
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

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