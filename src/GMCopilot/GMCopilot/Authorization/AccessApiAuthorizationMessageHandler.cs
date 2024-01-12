using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace GMCopilot.Authorization;

public class AccessApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    private readonly ILogger<AccessApiAuthorizationMessageHandler> _logger = default!;

    public AccessApiAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigationManager,
        ILogger<AccessApiAuthorizationMessageHandler> logger)
        : base(provider, navigationManager)
    {
        _logger = logger;

        logger.LogDebug($"Setting up {nameof(AccessApiAuthorizationMessageHandler)} to authorize the base url: {"https://localhost:7144"}");

        ConfigureHandler(
           authorizedUrls: new[] { "https://localhost:7144" },
           scopes: new[] { "api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.Read", "api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.ReadWrite" });
    }
}
