﻿using System.ComponentModel.DataAnnotations;

namespace GMCopilot.Core.Options;

public class EntraIdOptions
{
    // The settings key that contains the EntraId options
    public const string PropertyName = "EntraId";

    /// <summary>
    /// The base URL of the Entra ID authority endpoints. E.g., https://gmcopilot.ciamlogin.com
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string Instance { get; set; } = string.Empty;

    /// <summary>
    /// The base URL of the Entra ID authority endpoints. E.g., https://gmcopilot.ciamlogin.com/gmcopilot.onmicrosoft.com
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Domain name of the tenant, e.g., gmcopilot.onmicrosoft.com
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
    /// Application (client) secret
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Application Callback Path
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string CallbackPath { get; set; } = string.Empty;

    /// <summary>
    /// The SignUpSignIn policy ID if using Azure AD B2C/Entra ID External Identity
    /// </summary>
    [NotEmptyOrWhitespace]
    public string? SignUpSignInPolicyId { get; set; } = string.Empty;

    /// <summary>
    /// Whether to validate the authority
    /// </summary>
    public bool ValidateAuthority { get; set; } = false;
}
