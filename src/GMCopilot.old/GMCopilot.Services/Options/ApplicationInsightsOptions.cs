using System.ComponentModel.DataAnnotations;

namespace GMCopilot.Services.Options;

/// <summary>
/// Application Insights options for GMCopilot.
/// </summary>
public class ApplicationInsightsOptions
{
    public const string PropertyName = "ApplicationInsights";

    /// <summary>
    /// Connection String for Application Insights
    /// </summary>
    public string? ConnectionString { get; set; }

    [Required, NotEmptyOrWhitespace]
    public string? Test { get; set; }
}