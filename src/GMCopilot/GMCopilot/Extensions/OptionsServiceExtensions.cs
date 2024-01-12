using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using System.Reflection;
using GMCopilot.Core.Options;

namespace GMCopilot.Client.Extensions;

public static class OptionsServiceExtensions
{
    public static WebAssemblyHostBuilder AddOptions(this WebAssemblyHostBuilder builder)
    {
        // Add the Entra ID Configuration options
        builder.Services.AddOptions<EntraIdOptions>()
            .Bind(builder.Configuration.GetSection(EntraIdOptions.PropertyName))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        // Add the Apis options
        builder.Services.AddOptions<ApisOptions>()
            .Bind(builder.Configuration.GetSection(ApisOptions.PropertyName))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        return builder;
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