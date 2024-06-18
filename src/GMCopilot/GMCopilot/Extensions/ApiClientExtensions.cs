using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GMCopilot.Authorization;

namespace GMCopilot.Client.Extensions;

public static class ApiClientExtensions
{
    public static WebAssemblyHostBuilder AddApiClients(this WebAssemblyHostBuilder builder)
    {

        // var apiOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ApisOptions>>().Value;

        // Add the Authentication/Authorization for the APIs
        builder.Services.AddScoped<AccessApiAuthorizationMessageHandler>();
        
        var accessApiBaseAddress = new Uri(builder.Configuration["Apis:Access:BaseAddress"]);

        builder.Services.AddHttpClient("AccessApi",
                client => client.BaseAddress = accessApiBaseAddress)
            .AddHttpMessageHandler<AccessApiAuthorizationMessageHandler>();

        return builder;
    }
}
