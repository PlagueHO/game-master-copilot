using System.ComponentModel.DataAnnotations;

namespace DMCopilot.Services.Options;

/// <summary>
/// Configuration settings for connecting to Azure CosmosDB.
/// </summary>
public class CosmosDbOptions
{
    /// <summary>
    /// Cosmos DB endpoint URI
    /// </summary>
    [Required, Url]
    public string? EndpointUri { get; set; } = string.Empty;

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
    /// Cosmos DB container name for storing pages
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string? PagesContainerName { get; set; } = "pages";

    /// <summary>
    /// Cosmos DB container name for storing worlds
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string? WorldsContainerName { get; set; } = "worlds";
}
