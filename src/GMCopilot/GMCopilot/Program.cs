using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GMCopilot;
using GMCopilot.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AccessApiAuthorizationMessageHandler>();

builder.Services.AddHttpClient("AccessApi",
        client => client.BaseAddress = new Uri(builder.Configuration["Services:Access:BaseAddress"]))
    .AddHttpMessageHandler<AccessApiAuthorizationMessageHandler>();

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("EntraId", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("email");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("profile");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("User.Read");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.Read");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.ReadWrite");
});

builder.Services.AddMudServices();

await builder.Build().RunAsync();
