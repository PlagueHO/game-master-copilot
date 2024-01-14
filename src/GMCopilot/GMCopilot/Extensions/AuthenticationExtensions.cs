using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GMCopilot.Authorization;

namespace GMCopilot.Client.Extensions;

public static class AuthenticationExtensions
{
    public static WebAssemblyHostBuilder AddClientAuthentication(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddMsalAuthentication(options =>
        {
            builder.Configuration.Bind("EntraId", options.ProviderOptions.Authentication);
            options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
            options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
            options.ProviderOptions.DefaultAccessTokenScopes.Add("email");
            options.ProviderOptions.DefaultAccessTokenScopes.Add("profile");
            options.ProviderOptions.DefaultAccessTokenScopes.Add("api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.Read");
            options.ProviderOptions.DefaultAccessTokenScopes.Add("api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.ReadWrite");
        });

        return builder;
    }
}
