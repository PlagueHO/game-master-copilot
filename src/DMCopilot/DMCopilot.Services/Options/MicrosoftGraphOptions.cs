﻿using System.ComponentModel.DataAnnotations;

namespace DMCopilot.Services.Options;

/// <summary>
/// Microsoft Graph Configuration options for DMCopilot.
/// </summary>
public class MicrosoftGraphOptions
{
    public const string PropertyName = "MicrosoftGraph";

    /// <summary>
    /// BaseUrl for accessing the Microsoft Graph API
    /// </summary>
    [Url]
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Scopes to request from the Microsoft Graph API
    /// </summary>
    public string? Scopes { get; set; }
}