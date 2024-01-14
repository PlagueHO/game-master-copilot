using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace GMCopilot.Authorization;

public class AccessApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    /// <summary>
    /// The logger for the AccessApiAuthorizationMessageHandler class.
    /// </summary>
    private readonly ILogger<AccessApiAuthorizationMessageHandler> _logger;

    public AccessApiAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigationManager,
        ILogger<AccessApiAuthorizationMessageHandler> logger)
        : base(provider, navigationManager)
    {
        _logger = logger;

        var baseUri = new Uri("https://localhost:7144");
        _logger.LogDebug("Setting up {AccessApiAuthorizationHandler} to authorize the base url: {AccessApiBaseUrl}", nameof(AccessApiAuthorizationMessageHandler), baseUri.AbsoluteUri);

        ConfigureHandler(
           authorizedUrls: new[] { baseUri.AbsoluteUri },
           scopes: new[] { "api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.Read", "api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.ReadWrite" });
    }
}
