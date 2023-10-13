using System.ComponentModel.DataAnnotations;

namespace GMCopilot.Services.Options;

/// <summary>
/// App Configuration options for GMCopilot.
/// </summary>
public class AppConfigurationOptions
{
    public const string PropertyName = "AppConfiguration";

    /// <summary>
    /// App Configuration endpoint URI
    /// </summary>
    [Url]
    public string? Endpoint { get; set; }
}