# Game Master Copilot

[![continuous-integration](https://github.com/PlagueHO/game-master-copilot/actions/workflows/continuous-integration.yml/badge.svg)](https://github.com/PlagueHO/game-master-copilot/actions/workflows/continuous-integration.yml)[![continuous-deployment](https://github.com/PlagueHO/game-master-copilot/actions/workflows/continuous-deployment.yml/badge.svg)](https://github.com/PlagueHO/game-master-copilot/actions/workflows/continuous-deployment.yml)

Game Master Copilot is an AI enabled copilot to help game masters create content. It is a multitenant application that can be used to serve users and teams (organizations).

The application is a work-in-progress and is being used a test platform for various technologies and services. It is intended to demonstrate the use of Large Foundational Models/Generative AI using Azure OpenAI Service and [Semantic Kernel](https://aka.ms/sk/learn) as well as a reference architecture for running [multitenant SaaS applications in Azure](https://aka.ms/multitenantarchitecture).

This application uses the following Microsoft & Azure technologies, including:

- [Azure Container Apps](https://learn.microsoft.com/azure/container-apps/overview): Application and API hosting.
- [Azure OpenAI Service](https://learn.microsoft.com/azure/cognitive-services/openai/): foundational models (GPT-4 Turbo, DALL-E etc.)
- [Semantic Kernel](https://aka.ms/sk/learn): AI orchestration.
- [Azure AI Search](https://learn.microsoft.com/azure/search/search-what-is-azure-search): Search and semantic index (including vector support).
- [Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/introduction): Application data & document storage (including caching).
- [Azure Key Vault](https://learn.microsoft.com/azure/key-vault/general/overview): Key, secret and certificate handling.
- [Azure Blob Storage](https://learn.microsoft.com/azure/storage/blobs/storage-blobs-overview): Provides general file storage for the application.
- [Azure Application Insights](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview): Application monitoring.
- [.NET 8 Blazor WASM with Hosted Server](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor): Core application and APIs.
- [.NET 8 Aspire](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview): Application service discovery.

Future features planned:

- [Azure AD B2C](https://): Application identity.
- [Azure Application Configuration](https://learn.microsoft.com/azure/azure-app-configuration/overview): Implement external application configuration.
- [Azure Cache for Redis](https://learn.microsoft.com/azure/azure-cache-for-redis/cache-overview): Implement caching. Will use DAPR within the Azure Container Apps service.
- [Azure Event Grid](https://learn.microsoft.com/azure/event-grid/overview): Implement event mechansim to drive processes.

## Continuous Integration & Continuous Delivery

The application is built and deployed using:

- **Continuous integration** using [GitHub Actions workflow](https://docs.github.com/actions/using-workflows) from the main branch. See [continuous-integration.yml](https://github.com/PlagueHO/game-master-copilot/blob/main/.github/workflows/continuous-integraition.yml).
- **Continuous delivery** using [GitHub Actions workflow](https://docs.github.com/actions/using-workflows) from the main branch. See [continuous-deployment.yml](https://github.com/PlagueHO/game-master-copilot/blob/main/.github/workflows/continuous-deployment.yml).
- **Infrastrucutre as Code** using [Azure Bicep](). See [main.bicep](https://github.com/PlagueHO/game-master-copilot/blob/main/infrastructure/bicep/main.bicep).

### Workload Identity

The GitHub Actions workflow has been configured to use Entra ID Workload Identity for the workflow to connect to Azure. Please see [Configuring Workload Identity Federation for GitHub Actions workflow](#configuring-workload-identity-federation-for-github-actions-workflow) for more information.

## Environments

The following _environments_ should be configured in the GitHub repository:

- `TEST`: The Azure AD URL to use for authentication. For example, `https://login.microsoftonline.com/``.
- `PRODUCTION`: The Azure region to deploy the resources to. For example, `EastUS`.

Your environments should look similar to this:
![Example of GitHub Environments](/images/github-environments-example.png)

Currently the `continuous-delivery` only deploys the `test` environment, but in future it will deploy to both `test` and `production` depending on tagging and other criteria.

## Variables

The following _Actions Variables_ should be configured in the GitHub repository:

- `AZUREAD_INSTANCE`: The Azure AD URL to use for authentication. For example, `https://login.microsoftonline.com/`.
- `LOCATION`: The Azure region to deploy the resources to. For example, `EastUS`.
- `BASE_RESOURCE_NAME_SHARED`: The base name that will prefixed to all _shared_ Azure resources deployed to ensure they are unique. For example, `dsr-gmcopilot-shared`.
- `RESOURCE_GROUP_NAME_SHARED`: The name of the Azure resource group to create and add the _shared_ resources to. For example, `dsr-gmcopilot-shared-rg`.

The _shared_ resrouces are resources that are shared across all environments. These resources are:

- Azure Container Registry

### Environment Variables

Each _environment_ should have the following _actions variables_ defined:

- `BASE_RESOURCE_NAME`: The base name that will prefixed to all Azure resources deployed to ensure they are unique. For example, `dsr-gmcopilot-prod` for production.
- `RESOURCE_GROUP_NAME`: The name of the Azure resource group to create and add the application resources to. For example, `dsr-gmcopilot-prod-rg` for production.
- `ENVIRONMENT_CODE`: A code that will be used to identify this environment. For example, `prod` for production. _This is not currently used_.

Your variables should look similar to this:
![Example of GitHub Variables](/images/github-variables-example.png)

## Secrets

The following _Actions Secrets_ need to be defined so that that the resources can be deployed by the GitHub Actions workflow and that the Web Application can use Azure AD as an authentication source:

- `AZURE_CLIENT_ID`: The Application (Client) ID of the Service Principal used to authenticate to Azure. This is generated as part of configuring Workload Identity Federation.
- `AZURE_TENANT_ID`: The Tenant ID of the Service Principal used to authenticate to Azure.
- `AZURE_SUBSCRIPTION_ID`: The Subscription ID of the Azure Subscription to deploy to.
- `AZUREAD_DOMAIN`: The domain name of the Azure AD tenant the application will use as an authentication source.
- `AZUREAD_TENANT_ID`: The Tenant ID of the Azure AD tenant the application will use as an authentication source.
- `AZUREAD_CLIENT_ID`: The client ID of the Azure AD Application that has been created in the Azure AD tenant to be used as an authentication source.
- `AZUREAD_CLIENTSECRET`: The client secret of the Azure AD Application that has been created in the Azure AD tenant to be used as an authentication source.

These values should be kept secret and care taken to ensure they are not shared with anyone.
Your secrets should look like this:
![Example of GitHub Secrets](/images/github-secrets-example.png)
