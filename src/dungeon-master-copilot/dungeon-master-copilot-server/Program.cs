using Azure.Identity;
using dungeon_master_copilot_server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
.AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

// Get an Azure AD token for the application to use to authenticate to services in Azure
var azureCredential = new DefaultAzureCredential();

var semanticKernel = new KernelBuilder()
    .WithAzureTextCompletionService(
        builder.Configuration["AzureOpenAI:Deployment:Text"],
        builder.Configuration["AzureOpenAI:Endpoint"],
        azureCredential,
        "TextCompletion")
    .WithAzureChatCompletionService(
        builder.Configuration["AzureOpenAI:Deployment:Chat"],
        builder.Configuration["AzureOpenAI:Endpoint"],
        azureCredential,
        false,
        "ChatCompletion")
    .WithAzureTextEmbeddingGenerationService(
        builder.Configuration["AzureOpenAI:Deployment:TextEmbedding"],
        builder.Configuration["AzureOpenAI:Endpoint"],
        azureCredential,
        "Embeddings")
    .Build();

// Add the singleton service the abstracts the Semantic Kernel
builder.Services.AddSingleton<ISemanticKernelService>((svc) =>
{
    return new SemanticKernelService(semanticKernel);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
