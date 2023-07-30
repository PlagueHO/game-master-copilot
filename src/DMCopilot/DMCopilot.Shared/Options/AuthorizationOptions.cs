using System.ComponentModel.DataAnnotations;

namespace DMCopilot.Shared.Services.Options;

/// <summary>
/// Authorization options for DMCopilot.
/// </summary>
public class AuthorizationOptions
{
    public const string PropertyName = "Authorization";

    public enum AuthorizationType
    {
        None,
        AzureAd
    }

    public class AzureAdOptions
    {
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
        /// Application (client) secret
        /// </summary>
        [Required, NotEmptyOrWhitespace]
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// Application Callback Path
        /// </summary>
        [Required, NotEmptyOrWhitespace]
        public string CallbackPath { get; set; } = string.Empty;

        /// <summary>
        /// Required scopes.
        /// </summary>
        [Required]
        public string? Scopes { get; set; } = string.Empty;
    }


    /// <summary>
    /// Type of authorization.
    /// </summary>
    [Required]
    public AuthorizationType Type { get; set; } = AuthorizationType.None;

    /// <summary>
    /// When <see cref="Type"/> is <see cref="AuthorizationType.AzureAd"/>, these are the Azure AD options to use.
    /// </summary>
    [RequiredOnPropertyValue(nameof(Type), AuthorizationType.AzureAd)]
    public AzureAdOptions? AzureAd { get; set; }
}