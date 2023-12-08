var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.GMCopilot>("gmcopilot");

builder.Build().Run();
