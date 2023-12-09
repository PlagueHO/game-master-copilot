using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add Mudblazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
