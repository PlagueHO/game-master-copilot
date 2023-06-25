# Dungeon Master Copilot

[![continuous-deployment](https://github.com/PlagueHO/dungeon-master-copilot/actions/workflows/continuous-deployment.yml/badge.svg)](https://github.com/PlagueHO/dungeon-master-copilot/actions/workflows/continuous-deployment.yml)

Dungeon Master Copilot is an AI enabled copilot to help dungeon masters create content.

The application is a work-in-progress and is being used as both a test platform for various technologies and services, including:

- [Azure OpenAI Service](https://learn.microsoft.com/azure/cognitive-services/openai/): foundational models.
- [Semantic Kernel](https://aka.ms/sk/learn): AI orchestration.
- [Azure Cognitive Search](https://learn.microsoft.com/en-us/azure/search/search-what-is-azure-search): Search and semantic index (including private preview of vector index).
- [Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/introduction): Application data & document storage (including caching).
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview): Key, secret and certificate handling.
- [Azure Storage](): Provides general file storage for the application.
- [Azure AD](): Provides consumer facing identity services to the application (will be changed to AAD B2C).
- [.NET Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor): Application front-end.
- [.NET 7 with ASP.NET Core 7](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-7.0?view=aspnetcore-7.0): Application backend.

Future:

- [Azure Application Configuration](): Implement external application configuration.
- [Azure Cache for Redis](): Implement external caching.
- [Azure Event Grid](): Implement event mechansim to drive processes.
- [Azure Functions](): Move APIs out of backend.

## Build and Deployment

The application is built and deployed using the following:

- Continuous delivery using [GitHub Actions workflow]() from main branch. See [continuous-deployment.yml](https://github.com/PlagueHO/dungeon-master-copilot/blob/main/.github/workflows/continuous-deployment.yml)
- Infrastrucutre as Code using [Azure Bicep](). See [main.bicep](https://github.com/PlagueHO/dungeon-master-copilot/blob/main/infrastructure/bicep/main.bicep).

Planned:

- Once initial version has been deployed, main branch will be protected via branch policy and only accept PRs. PRs will require passing checks etc.
- Add a Deployment to a Test environment before production will be enabled, which will require changes to infrastructure resource names.
