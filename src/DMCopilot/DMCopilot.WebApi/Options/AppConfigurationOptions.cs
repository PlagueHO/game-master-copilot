using System.ComponentModel.DataAnnotations;

namespace DMCopilot.WebApi.Service;

/// <summary>
/// App Configuration Configuration options for the DMCopilot.WebApi.
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
