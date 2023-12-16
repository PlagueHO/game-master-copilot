using System.ComponentModel.DataAnnotations;

namespace GMCopilot.Core.Options;

/// <summary>
/// Data Store Configuration options for GMCopilot.
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
    /// Gets or sets the type of data store to use.
    /// </summary>
    public DataStoreType Type { get; set; } = DataStoreType.CosmosDb;

    /// <summary>
    /// Gets or sets the configuration for the Azure CosmosDB data store.
    /// </summary>
    [RequiredOnPropertyValue(nameof(Type), DataStoreType.CosmosDb)]
    public CosmosDbOptions? CosmosDb { get; set; } = new CosmosDbOptions();
}