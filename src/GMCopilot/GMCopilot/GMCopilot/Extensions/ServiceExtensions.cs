using GMCopilot.Data.Repositories;
using GMCopilot.Entities.Models;
using GMCopilot.Entities.Options;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace GMCopilot.Backend.Extensions;

public static class BackendServiceExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
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
        services
            .AddSingleton<ILogger>(sp => sp.GetRequiredService<ILogger<Program>>()) // some services require an un-templated ILogger
            .AddSingleton<ILoggerFactory, LoggerFactory>()
            .AddHttpContextAccessor()
            .AddLogging(logBuilder =>
            {
                logBuilder.AddConsole();
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
        IStorageContext<Universe> universeStorageContext;

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
                    universeStorageContext = new CosmosDbContext<Universe>(
                        dataStoreConfig.CosmosDb.ConnectionString, dataStoreConfig.CosmosDb.DatabaseName, dataStoreConfig.CosmosDb.UniversesContainerName);
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
        services.AddSingleton<UniverseRepository>(new UniverseRepository(universeStorageContext));

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