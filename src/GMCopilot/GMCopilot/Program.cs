using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GMCopilot.Client.Extensions;

namespace GMCopilot.Client;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        
        // Add the Options
        builder.AddClientOptions();

        // Add the Authentication/Authorization
        builder.AddClientAuthentication();

        // Add the API Clients
        builder.AddApiClients();

        // Add the MudBlazor services
        builder.Services.AddMudServices();

        await builder.Build().RunAsync();
    }
}