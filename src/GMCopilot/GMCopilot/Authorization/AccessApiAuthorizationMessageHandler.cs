using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace GMCopilot.Authorization;

public class AccessApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public AccessApiAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigationManager)
        : base(provider, navigationManager)
    {
        ConfigureHandler(
           authorizedUrls: new[] { "https://localhost:7144" },
           scopes: new[] { "api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.Read", "api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.ReadWrite" });
    }
}
