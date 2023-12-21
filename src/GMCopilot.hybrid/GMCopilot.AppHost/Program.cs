var builder = DistributedApplication.CreateBuilder(args);

var applicationInsightsConnectionString =
    builder.Configuration["ApplicationInsights:ConnectionString"];

if (applicationInsightsConnectionString == null)
{
    throw new InvalidOperationException("ApplicationInsights:ConnectionString is required.");
}

var dataStoreCosmosDbConnectionString =
    builder.Configuration["DataStore:CosmosDb:ConnectionString"];

if (dataStoreCosmosDbConnectionString == null)
{
    throw new InvalidOperationException("DataStore:CosmosDb:ConnectionString is required.");
}

// Add the Application project
builder.AddProject<Projects.GMCopilot>("gmcopilot")
    .WithEnvironment("ApplicationInsights:ConnectionString",applicationInsightsConnectionString);

// Add the Access API project
builder.AddProject<Projects.GMCopilot_AccessApi>("gmcopilot.accessapi")
    .WithEnvironment("ApplicationInsights:ConnectionString", applicationInsightsConnectionString)
    .WithEnvironment("DataStore:CosmosDb:ConnectionString", dataStoreCosmosDbConnectionString);

builder.Build().Run();
