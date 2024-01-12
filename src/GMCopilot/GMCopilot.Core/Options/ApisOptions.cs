using System.ComponentModel.DataAnnotations;

namespace GMCopilot.Core.Options;

public class ApisOptions
{
    // The settings key that contains the API options
    public const string PropertyName = "Apis";

    /// <summary>
    /// The scopes to request from the Entra ID authority
    /// </summary>
    public Dictionary<string, ApiOptions> Apis { get; set; } = [];
}

public class ApiOptions
{
    /// <summary>
    /// The base URL of the service. E.g., https://localhost:7144
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public Uri? BaseAddress { get; set; }

    /// <summary>
    /// The scope that will be requested from the IDP. E.g., api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.Read
    /// </summary>
    public Uri? Scope { get; set; }
}
