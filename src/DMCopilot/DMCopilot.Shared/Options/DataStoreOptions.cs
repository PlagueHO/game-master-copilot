using System.ComponentModel.DataAnnotations;

namespace DMCopilot.Shared.Services.Options;

/// <summary>
/// Data Store Configuration options for DMCopilot.
/// </summary>
public class DataStoreOptions
{
    public const string PropertyName = "DataStore";

    /// <summary>
    /// The type of chat store to use.
    /// </summary>
    public enum DataStoreType
    {
        /// <summary>
        /// Azure Cosmos DB based persistent chat store.
        /// </summary>
        CosmosDb
    }

    /// <summary>
    /// Configuration settings for connecting to Azure CosmosDB.
    /// </summary>
    public class CosmosDbOptions
    {
        /// <summary>
        /// Cosmos DB endpoint URI
        /// </summary>
        [Required, Url]
        public string? Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// Cosmos DB database name
        /// </summary>
        public string? DatabaseName { get; set; } = "dmcopilot";

        /// <summary>
        /// Cosmos DB connection string
        /// </summary>
        [Required, NotEmptyOrWhitespace]
        public string? ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Cosmos DB container name for storing accounts
        /// </summary>
        [Required, NotEmptyOrWhitespace]
        public string? AccountsContainerName { get; set; } = "accounts";

        /// <summary>
        /// Cosmos DB container name for storing tenants
        /// </summary>
        [Required, NotEmptyOrWhitespace]
        public string? TenantsContainerName { get; set; } = "tenants";

        /// <summary>
        /// Cosmos DB container name for storing worlds
        /// </summary>
        [Required, NotEmptyOrWhitespace]
        public string? WorldsContainerName { get; set; } = "worlds";

        /// <summary>
        /// Cosmos DB container name for storing characters
        /// </summary>
        [Required, NotEmptyOrWhitespace]
        public string? CharactersContainerName { get; set; } = "characters";
    }

    /// <summary>
    /// Gets or sets the type of chat store to use.
    /// </summary>
    public DataStoreType Type { get; set; } = DataStoreType.CosmosDb;

    /// <summary>
    /// Gets or sets the configuration for the Azure CosmosDB chat store.
    /// </summary>
    [RequiredOnPropertyValue(nameof(Type), DataStoreType.CosmosDb)]
    public CosmosDbOptions? CosmosDb { get; set; } = new CosmosDbOptions();

}