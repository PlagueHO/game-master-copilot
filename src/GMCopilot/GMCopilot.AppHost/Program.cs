using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add Cosmos DB to the application
var database = builder.AddAzureCosmosDB("CosmosDb")
    .PublishAsConnectionString()
    .RunAsEmulator()
    .AddDatabase("gmcopilot");

// Add the Access API project
builder.AddProject<Projects.GMCopilot_AccessApi>("accessapi")
    .WithReference(database);

builder.Build().Run();
