using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GMCopilot;
using GMCopilot.Authorization;
using GMCopilot.Client.Extensions;
using Microsoft.Graph.Models.Security;

namespace GMCopilot.Client;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.Services.AddOptions();

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

        // Add the Authentication/Authorization for the APIs
        builder.Services.AddScoped<AccessApiAuthorizationMessageHandler>();

        builder.Services.AddHttpClient("AccessApi",
                client => client.BaseAddress = new Uri(builder.Configuration["Apis:Access:BaseAddress"]))
            .AddHttpMessageHandler<AccessApiAuthorizationMessageHandler>();

        builder.Services.AddMudServices();

        await builder.Build().RunAsync();
    }
}