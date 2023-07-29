using System.ComponentModel.DataAnnotations;

namespace DMCopilot.WebApi.Service;

/// <summary>
/// Cosmos DB Configuration options for the DMCopilot.WebApi.
/// </summary>
public class AzureAdOptions

{
    public const string PropertyName = "AzureAd";

    /// <summary>
    /// AAD instance url, i.e., https://login.microsoftonline.com/
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string Instance { get; set; } = string.Empty;

    /// <summary>
    /// Domain
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Tenant (directory) ID
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Application (client) ID
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Application (client) ID
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string CallbackPath { get; set; } = string.Empty;

    /// <summary>
    /// Required scopes.
    /// </summary>
    [Required]
    public string? Scopes { get; set; } = string.Empty;
}
