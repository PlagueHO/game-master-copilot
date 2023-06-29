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

The application is built and deployed using:

- **Continuous delivery** using [GitHub Actions workflow]() from the main branch. See [continuous-deployment.yml](https://github.com/PlagueHO/dungeon-master-copilot/blob/main/.github/workflows/continuous-deployment.yml).
- **Infrastrucutre as Code** using [Azure Bicep](). See [main.bicep](https://github.com/PlagueHO/dungeon-master-copilot/blob/main/infrastructure/bicep/main.bicep).

### Workload Identity

The GitHub Actions workflow has been configured to use Azure AD Workload Identity for the workflow to connect to Azure. Please see [Configuring Workload Identity Federation for GitHub Actions workflow](#configuring-workload-identity-federation-for-github-actions-workflow) for more information.

The following _Actions Variables_ should be configured in the GitHub repository:

- `APPSERVICEPLAN_CONFIGURATION`: The configuration of the Azure App Service Plan for running the Foundry VTT server. Must be one of `B1`, `P1V2`, `P2V2`, `P3V2`, `P0V3`,`P1V3`, `P2V3`, `P3V3`.
- `AZUREAD_INSTANCE`: The Azure AD URL to use for authentication. For example, 'https://login.microsoftonline.com/'.
- `LOCATION`: The Azure region to deploy the resources to. For example, `EastUS`.
- `BASE_RESOURCE_NAME`: The base name that will prefixed to all Azure resources deployed to ensure they are unique. For example, `dsr-dmcopilot`.
- `RESOURCE_GROUP_NAME`: The name of the Azure resource group to create and add the resources to. For example, `dsr-dmcopilot-rg`.

Your variables should look similar to this:
![Example of GitHub Variables](/images/github-variables-example.png)

The following _Actions Secrets_ need to be defined so that that the resources can be deployed by the GitHub Actions workflow and that the Web Application can use Azure AD as an authentication source:

- `AZURE_CLIENT_ID`: The Application (Client) ID of the Service Principal used to authenticate to Azure. This is generated as part of configuring Workload Identity Federation.
- `AZURE_TENANT_ID`: The Tenant ID of the Service Principal used to authenticate to Azure.
- `AZURE_SUBSCRIPTION_ID`: The Subscription ID of the Azure Subscription to deploy to.
- `AZUREAD_DOMAIN`: The domain name of the Azure AD tenant the application will use as an authentication source.
- `AZUREAD_TENANTID`: The Tenant ID of the Azure AD tenant the application will use as an authentication source.
- `AZUREAD_CLIENT_ID`: The client ID of the Azure AD Application that has been created in the Azure AD tenant to be used as an authentication source.
- `AZUREAD_CLIENTSECRET`: The client secret of the Azure AD Application that has been created in the Azure AD tenant to be used as an authentication source.

These values should be kept secret and care taken to ensure they are not shared with anyone.
Your secrets should look like this:
![Example of GitHub Secrets](/images/github-secrets-example.png)
