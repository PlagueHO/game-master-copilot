using System.ComponentModel.DataAnnotations;

namespace DMCopilot.WebApi.Service;

/// <summary>
/// Cosmos DB Configuration options for the DMCopilot.WebApi.
/// </summary>
public class CosmosDbOptions

{
    public const string PropertyName = "CosmosDb";

    /// <summary>
    /// Cosmos DB endpoint URI
    /// </summary>
    [Url]
    public string? Endpoint { get; set; }

    /// <summary>
    /// Cosmos DB database name
    /// </summary>
    public string? DatabaseName { get; set; }
}
