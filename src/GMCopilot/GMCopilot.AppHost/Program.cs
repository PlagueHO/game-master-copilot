var builder = DistributedApplication.CreateBuilder(args);

// Add Cosmos DB to the application
var cosmosDb = builder.AddAzureCosmosDB("CosmosDb");
var database = cosmosDb.AddDatabase("gmcopilot");

// Add the Access API project
builder.AddProject<Projects.GMCopilot_AccessApi>("gmcopilot-accessapi")
    .WithReference(database);

builder.Build().Run();
