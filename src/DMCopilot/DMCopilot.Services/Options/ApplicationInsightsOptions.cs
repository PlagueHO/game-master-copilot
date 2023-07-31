using System.ComponentModel.DataAnnotations;

namespace DMCopilot.Services.Options;

/// <summary>
/// Application Insights options for DMCopilot.
/// </summary>
public class ApplicationInsightsOptions
{
    public const string PropertyName = "ApplicationInsights";

    /// <summary>
    /// Connection String for Application Insights
    /// </summary>
    public string? ConnectionString { get; set; }
}