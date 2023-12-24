using Microsoft.Azure.Cosmos;
using GMCopilot.Core.Models;
using GMCopilot.Data.Repositories;

namespace GMCopilot.AccessApi.Extensions;

public static class DataStoreServiceExtensions
{
    /// <summary>
    /// Add data store services.
    /// </summary>
    public static IHostApplicationBuilder AddDataStore(this IHostApplicationBuilder builder)
    {
        // Create the Cosmos DB Client
        var dataStoreCosmosDbConnectionString =
            builder.Configuration["DataStore:CosmosDb:ConnectionString"];

        if (dataStoreCosmosDbConnectionString == null)
        {
            throw new InvalidOperationException("DataStore:CosmosDb:ConnectionString is required.");
        }

        builder.AddAzureCosmosDB(dataStoreCosmosDbConnectionString,
            configureClientOptions: clientOptions => clientOptions.ApplicationName = "game-master-copilot-access-api");

        // Create the Account context and repository
        builder.Services.AddScoped<CosmosDbContext<Account>>((service) =>
        {
            var cosmosClient = service.GetService<CosmosClient>();
            if (cosmosClient == null)
            {
                throw new InvalidOperationException("CosmosClient could not be instantiated.");
            }

            return new CosmosDbContext<Account>(cosmosClient, "gmcopilot", "accounts");
        });

        builder.Services.AddScoped<AccountRepository>((service) =>
        {
            var accountStorageContext = service.GetService<CosmosDbContext<Account>>();
            if (accountStorageContext == null)
            {
                throw new InvalidOperationException("CosmosDbContext<Account> could not be instantiated.");
            }

            return new AccountRepository(accountStorageContext);
        });

        // Create the Tenant context and repository
        builder.Services.AddScoped<CosmosDbContext<Tenant>>((service) =>
        {
            var cosmosClient = service.GetService<CosmosClient>();
            if (cosmosClient == null)
            {
                throw new InvalidOperationException("CosmosClient could not be instantiated.");
            }

            return new CosmosDbContext<Tenant>(cosmosClient, "gmcopilot", "tenants");
        });


        builder.Services.AddScoped<AccountRepository>((service) =>
        {
            var accountStorageContext = service.GetService<CosmosDbContext<Account>>();
            if (accountStorageContext == null)
            {
                throw new InvalidOperationException("CosmosDbContext<Account> could not be instantiated.");
            }

            return new AccountRepository(accountStorageContext);
        });

        return builder;
    }
}