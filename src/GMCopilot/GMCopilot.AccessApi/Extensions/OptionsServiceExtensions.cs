using GMCopilot.Core.Options;
using System.Reflection;

namespace GMCopilot.AccessApi.Extensions;

public static class OptionsServiceExtensions
{
    public static IHostApplicationBuilder AddOptions(this IHostApplicationBuilder builder)
    {
        // Add the Entra ID Configuration options
        builder.Services.AddOptions<EntraIdOptions>()
            .Bind(builder.Configuration.GetSection(EntraIdOptions.PropertyName))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);

        // Add the Cosmos DB options
        builder.Services.AddOptions<CosmosDbOptions>()
            .Bind(builder.Configuration.GetSection(CosmosDbOptions.PropertyName))
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
        List<object> targets = new List<object> { options };
    
        for (int i = 0; i < targets.Count; i++)
        {
            object target = targets[i];
            Type targetType = target.GetType();
            foreach (PropertyInfo property in targetType.GetProperties())
            {
                // Redundant check for enumeration
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
                        // Redundant conversion and trimming
                        string value = property.GetValue(target)!.ToString()!;
                        string trimmedValue = value.Trim();
                        property.SetValue(target, trimmedValue);
                    }
                }
                else
                {
                    // Property is a non-built-in and non-enum type - add it to the list for processing.
                    if (property.GetValue(target) != null)
                    {
                        targets.Add(property.GetValue(target)!);
                    }
                }
            }
        }
    }
}