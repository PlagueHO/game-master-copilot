# Dungeon Master Copilot

[![continuous-deployment](https://github.com/PlagueHO/dungeon-master-copilot/actions/workflows/continuous-deployment.yml/badge.svg)](https://github.com/PlagueHO/dungeon-master-copilot/actions/workflows/continuous-deployment.yml)

Dungeon Master Copilot is an AI enabled copilot to help dungeon masters create content. It is a multitenant application that can be used to serve users and teams (organizations).

The application is a work-in-progress and is being used a test platform for various technologies and services. It is intended to demonstrate the use of Large Foundational Models/Generative AI using Azure OpenAI Service and [Semantic Kernel](https://aka.ms/sk/learn) as well as a reference architecture for running [multitenant SaaS applications in Azure](https://aka.ms/multitenantarchitecture).

This application uses the following Microsoft & Azure technologies, including:

- [Azure OpenAI Service](https://learn.microsoft.com/azure/cognitive-services/openai/): foundational models.
- [Semantic Kernel](https://aka.ms/sk/learn): AI orchestration.
- [Azure Cognitive Search](https://learn.microsoft.com/en-us/azure/search/search-what-is-azure-search): Search and semantic index (including private preview of vector index).
- [Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/introduction): Application data & document storage (including caching).
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview): Key, secret and certificate handling.
- [Azure Blob Storage](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-overview): Provides general file storage for the application.
- [Azure AD External Identities](https://learn.microsoft.com/en-us/azure/active-directory/external-identities/external-identities-overview): Provides consumer facing identity services to the application (will be changed to Azure AD B2C in future).
- [Azure Application Insights](https://)
- [.NET Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor): Application front-end.
- [.NET 7 with ASP.NET Core 7](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-7.0?view=aspnetcore-7.0): Application backend.

Future features planned:

- [Azure Application Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview): Implement external application configuration.
- [Azure Cache for Redis](https://learn.microsoft.com/en-us/azure/azure-cache-for-redis/cache-overview): Implement caching.
- [Azure Event Grid](https://learn.microsoft.com/en-us/azure/event-grid/overview): Implement event mechansim to drive processes.
- [Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview): Move APIs out of backend.
- [Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/overview): Break application down into more distributed components.

## Build and Deployment

The application is built and deployed using the following:

- Continuous delivery using [GitHub Actions workflow]() from the main branch. See [continuous-deployment.yml](https://github.com/PlagueHO/dungeon-master-copilot/blob/main/.github/workflows/continuous-deployment.yml)
- Infrastrucutre as Code using [Azure Bicep](). See [main.bicep](https://github.com/PlagueHO/dungeon-master-copilot/blob/main/infrastructure/bicep/main.bicep).

Planned:

- Once initial version has been deployed, main branch will be protected via branch policy and only accept PRs. PRs will require passing checks etc.
- Add a Deployment to a Test environment before production will be enabled, which will require changes to infrastructure resource names.
