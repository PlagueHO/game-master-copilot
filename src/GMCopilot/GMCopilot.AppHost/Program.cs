var builder = DistributedApplication.CreateBuilder(args);

var applicationInsightsConnectionString =
    builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

builder.AddProject<Projects.GMCopilot>("gmcopilot").WithEnvironment(
        "APPLICATIONINSIGHTS_CONNECTION_STRING",
        applicationInsightsConnectionString);

builder.Build().Run();
