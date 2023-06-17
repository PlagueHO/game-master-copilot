# Dungeon Master Copilot

Dungeon Master Copilot is an AI enabled copilot to help dungeon masters create content.

The application is a work-in-progress and is being used as both a test platform for various technologies and services, including:

- [Azure OpenAI Service](https://learn.microsoft.com/azure/cognitive-services/openai/): foundational models.
- [Semantic Kernel](https://aka.ms/sk/learn): AI orchestration.
- [Azure Cognitive Services](): Semantic index (including Vector Index)
- [Azure Cosmos DB](): Document storage.
- [Azure AD](): Provides identity services to the application.
- [.NET 7.0 Blazor](): Application server.

## Application components

The application tier consists of a server component build with .NET 7.0 Blazor.

The application also uses the following services:

- Azure App Service: hosting the .NET 7.0 Blazor application.
- Azure OpenAI Service: providing foundational models.
- Azure Storage: general storage for the application.
- Azure Log Analytics: diagnostic logs for the other components.
- Azure Application Insights: application performance monitoring.
