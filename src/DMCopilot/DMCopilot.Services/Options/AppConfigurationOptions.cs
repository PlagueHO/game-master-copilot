using System.ComponentModel.DataAnnotations;

namespace DMCopilot.Services.Options;

/// <summary>
/// App Configuration options for DMCopilot.
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