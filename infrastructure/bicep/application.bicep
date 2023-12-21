targetScope = 'subscription'

@description('The location to deploy the Game Master Copilot into.')
@allowed([
  'AustraliaEast'
  'CanadaEast'
  'EastUS'
  'EastUS2'
  'FranceCentral'
  'JapanEast'
  'NorthCentralUS'
  'SouthCentralUS'
  'SwedenCentral'
  'SwitzerlandNorth'
  'WestEurope'
  'UKSouth'
])
param location string = 'CanadaEast'

@description('The location to deploy the Game Master Copilot static web app into.')
@allowed([
  'CentralUS'
  'EastUS2'
  'EastAsia'
  'WestEurope'
  'WestUS2'
])
param staticWebAppLocation string = 'CentralUS'

@description('The environment to deploy the Game Master Copilot into.')
@allowed([
  'Test'
  'Production'
])
param environmentCode string = 'Test'

@description('The base name that will prefixed to all Azure resources deployed to ensure they are unique.')
param baseResourceName string

@description('The name of the resource group that will contain all the resources.')
param resourceGroupName string

@description('The name of the resource group that contains shared resources (e.g., Container Registry).')
param resourceGroupNameShared string

@description('The base name of the shared resources')
param baseResourceNameShared string

@description('The build version to publish to the components.')
param buildVersion string

@description('The Entra ID issuer URL to use for authentication.')
param entraIdIssuerUrl string

@description('The Entra ID tenant ID to use for authentication.')
@secure()
param entraIdTenantId string

@description('The domain name of the Entra ID tenant')
@secure()
param entraIdDomain string

@description('The client ID that the WASM client will used to connect to the Entra ID to perform application authentication.')
@secure()
param entraIdClientIdClient string

@description('The client ID that the APIs will used to connect to the Entra ID to perform application authentication.')
@secure()
param entraIdClientIdApi string


var logAnalyticsWorkspaceName = '${baseResourceName}-law'
var applicationInsightsName = '${baseResourceName}-ai'
var keyVaultName = '${baseResourceName}-akv'
var staticWebAppName = baseResourceName
var appConfigurationName = '${baseResourceName}-appconfig'
var openAiServiceName = '${baseResourceName}-oai'
var aiSearchName = '${baseResourceName}-aisearch'
var cosmosDbAccountName = '${baseResourceName}-cdb'
var storageAccountName = replace('${baseResourceName}data','-','')
var containerAppEnvironmentName = '${baseResourceName}-cae'
var containerAppName = baseResourceName
var containerAppUserAssignedManagedIdentityName = '${baseResourceName}-caumi'
var containerRegistryName = replace('${baseResourceNameShared}acr','-','')

var openAiModelDeployments = [
  {
    name: 'gpt-35-turbo'
    modelName: 'gpt-35-turbo'
    version: '1106'
    sku: 'Standard'
    capacity: 50
  }
  {
    name: 'gpt-4'
    modelName: 'gpt-4'
    version: '1106-Preview'
    sku: 'Standard'
    capacity: 20
  }
  {
    name: 'text-embedding-ada-002'
    modelName: 'text-embedding-ada-002'
    version: '2'
    sku: 'Standard'
    capacity: 50
  }
]

// Shared resources that are deployed into a shared resource group
resource rgshared 'Microsoft.Resources/resourceGroups@2021-04-01' existing = {
  name: resourceGroupNameShared
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-01-01-preview' existing = {
  name: containerRegistryName
  scope: rgshared
}

var containerRegistryLoginServer = containerRegistry.properties.loginServer

// The application resources that are deployed into the application resource group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

module monitoring './modules/monitoring.bicep' = {
  name: 'monitoring'
  scope: rg
  params: {
    location: location
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    applicationInsightsName: applicationInsightsName
  }
}

module keyVault './modules/keyVault.bicep' = {
  name: 'keyVault'
  scope: rg
  params: {
    location: location
    keyVaultName: keyVaultName
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
  }
}

module appConfiguration './modules/appConfiguration.bicep' = {
  name: 'appConfiguration'
  scope: rg
  params: {
    location: location
    appConfigurationName: appConfigurationName
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
  }
}

module cosmosDbAccount './modules/cosmosDbAccount.bicep' = {
  name: 'cosmosDbAccount'
  scope: rg
  params: {
    location: location
    cosmosDbAccountName: cosmosDbAccountName
  }
}

module openAiService './modules/openAiService.bicep' = {
  name: 'openAiService'
  scope: rg
  dependsOn: [
    monitoring
  ]
  params: {
    location: location
    openAiServiceName: openAiServiceName
    openAiModeldeployments: openAiModelDeployments
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
  }
}

module aiSearch './modules/aiSearch.bicep' = {
  name: 'aiSearch'
  scope: rg
  dependsOn: [
    monitoring
  ]
  params: {
    location: location
    aiSearchName: aiSearchName
    sku: 'basic'
    replicaCount: 1
    partitionCount: 1
    hostingMode: 'default'
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
  }
}

module storageAccount './modules/storageAccount.bicep' = {
  name: 'storageAccount'
  scope: rg
  params: {
    location: location
    storageAccountName: storageAccountName
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
  }
}

module containerAppEnvironment './modules/containerAppEnvironment.bicep' = {
  name: 'containerAppEnvironment'
  scope: rg
  params: {
    location: location
    containerAppEnvironmentName: containerAppEnvironmentName
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
  }
}

module containerAppUserAssignedManagedIdentity './modules/userAssignedManagedIdentity.bicep' = {
  name: 'containerAppUserAssignedManagedIdentity'
  scope: rg
  params: {
    location: location
    userAssignedManagedIdentityName: containerAppUserAssignedManagedIdentityName
  }
}

// Define the object that contains chunks of configuration information that can be used by each container app
var containerAppEnvrionmentVariables = {
  Default: [
    {
      name: 'ASPNETCORE_ENVIRONMENT'
      value: environmentCode == 'Production' ? 'Production' : 'Development'
    }
    {
      name: 'ApplicationInsights__ConnectionString'
      secretRef: 'applicationinsights-connectionstring'
    }
    {
      name: 'DetailedErrors'
      value: 'true'
    }
    {
      name: 'Logging__LogLevel__Default'
      value: 'Information'
    }
    {
      name: 'Logging__LogLevel__Microsoft.AspNetCore'
      value: 'Warning'
    }
  ]
  Authentication: [
    {
      name: 'EntraId__IssurerUrl'
      value: entraIdIssuerUrl
    }
    {
      name: 'EntraId__TenantId'
      value: entraIdTenantId
    }
    {
      name: 'EntraId__Domain'
      value: entraIdDomain
    }
    {
      name: 'EntraId__ClientId'
      value: entraIdClientIdApi
    }
    {
      name: 'EntraId__CallbackPath'
      value: '/signin-oidc'
    }
    {
      name: 'EntraId__Scopes'
      value: '/api_access'
    }
  ]
  SemanticKernel: [
    {
      name: 'SemanticKernel__PluginsDirectory'
      value: 'Plugins'
    }
    {
      name: 'SemanticKernel__AzureOpenAiApiKey'
      secretRef: 'semantickernel-azureopenaiapikey'
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextCompletionServices__0__Id'
      value: 'ChatCompletionGPT35TURBO'
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextCompletionServices__0__Endpoint'
      value: openAiService.outputs.openAiServiceEndpoint
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextCompletionServices__0__Deployment'
      value: openAiModelDeployments[0].name
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextCompletionServices__0__SetAsDefault'
      value: 'true'
    }
    {
      name: 'SemanticKernel__AzureOpenAiChatCompletionServices__0__AlsoAsTextCompletion'
      value: 'true'
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextCompletionServices__1__Id'
      value: 'ChatCompletionGPT4'
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextCompletionServices__1__Endpoint'
      value: openAiService.outputs.openAiServiceEndpoint
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextCompletionServices__1__Deployment'
      value: openAiModelDeployments[1].name
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextCompletionServices__1__SetAsDefault'
      value: 'false'
    }
    {
      name: 'SemanticKernel__AzureOpenAiChatCompletionServices__1__AlsoAsTextCompletion'
      value: 'true'
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextEmbeddingGenerationServices__2__Id'
      value: 'Embeddings'
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextEmbeddingGenerationServices__2__Endpoint'
      value: openAiService.outputs.openAiServiceEndpoint
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextEmbeddingGenerationServices__2__Deployment'
      value: openAiModelDeployments[1].name
    }
    {
      name: 'SemanticKernel__AzureOpenAiTextEmbeddingGenerationServices__2__SetAsDefault'
      value: 'true'
    }
    {
      name: 'SemanticKernel__AzureOpenAiImageGenerationServices__3__Id'
      value: 'ImageGeneration'
    }
    {
      name: 'SemanticKernel__AzureOpenAiImageGenerationServices__3__Endpoint'
      value: openAiService.outputs.openAiServiceEndpoint
    }
    {
      name: 'SemanticKernel__AzureOpenAiImageGenerationServices__3__SetAsDefault'
      value: 'true'
    }
  ]
  DataStore: [
    {
      name: 'DataStore__Type'
      value: 'CosmosDb'
    }
    {
      name: 'DataStore__CosmosDb__Database'
      value: 'gmcopilot'
    }
    {
      name: 'DataStore__CosmosDb__ConnectionString'
      secretRef: 'datastore-cosmosdb-connectionstring'
    }
  ]
}

// Define the objects (containers and secrets) used to create for the Container App Web (front end)
resource cosmosDbAccountExisting 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbAccountName
  scope: rg
}

resource openAiServiceExisting 'Microsoft.CognitiveServices/accounts@2023-05-01' existing = {
  name: openAiServiceName
  scope: rg
}

var containerAppWebSecrets = [
  {
    name: 'datastore-cosmosdb-connectionstring'
    value: cosmosDbAccountExisting.listConnectionStrings().connectionStrings[0].connectionString
  }
  {
    name: 'applicationinsights-connectionstring'
    value: monitoring.outputs.applicationInsightsConnectionString
  }
  {
    name: 'semantickernel-azureopenaiapikey'
    value: openAiServiceExisting.listKeys().key1
  }
]

var containerAppWebContainers = [
  {
    name: 'gmcopilot-access-api'
    image: '${containerRegistryLoginServer}/gmcopilot/gmcopilot.accessapi:${buildVersion}'
    probes: [
      {
        type: 'liveness'
        httpGet: {
          path: '/health'
          port: 8080
          httpHeaders: [
            {
              name: 'Custom-Header'
              value: 'liveness probe'
            }
          ]
        }
        initialDelaySeconds: 7
        periodSeconds: 10
        timeoutSeconds: 1
        successThreshold: 1
        failureThreshold: 3
      }
      {
        type: 'readiness'
        httpGet: {
          path: '/alive'
          port: 8080
        }
        initialDelaySeconds: 7
        periodSeconds: 5
        timeoutSeconds: 5
        successThreshold: 1
        failureThreshold: 3
      }
    ]
    resources: {
      cpu: json('0.25')
      memory: '0.5Gi'
    }
    env: union(containerAppEnvrionmentVariables.Default, containerAppEnvrionmentVariables.DataStore)
  }
]

module containerAppWeb './modules/containerApp.bicep' = {
  name: 'containerAppWeb'
  scope: rg
  params: {
    location: location
    containerAppName: containerAppName
    containerRegistryLoginServer: containerRegistryLoginServer
    userAssignedManagedIdentityName: containerAppUserAssignedManagedIdentityName
    containerAppEnvironmentName: containerAppEnvironmentName
    containers: containerAppWebContainers
    secrets: containerAppWebSecrets
  }
}

module staticWebApp './modules/staticWebApp.bicep' = {
  name: 'staticWebApp'
  scope: rg
  params: {
    location: staticWebAppLocation
    staticWebAppName: staticWebAppName
  }
}

// Configure all the RBAC roles to allow the services to access each other
var roles = {
    'Cognitive Services OpenAI User': '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
    'Storage Blob Data Contributor': 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
    'Cosmos DB Account Reader Role': 'fbdf93bf-df7d-467e-a4d2-9458aa1360c8'
    'Key Vault Secrets User': '4633458b-17de-408a-b874-0445c86b69e6'
    'App Configuration Data Reader': '516239f1-63e1-4d78-a4de-a74fb236a071'
    AcrPull: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
}

module keyVaultAppConfigurationRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'keyVaultAppConfigurationRoleServicePrincipal'
  params: {
    principalId: appConfiguration.outputs.appConfigurationIdentityPrincipalId
    roleDefinitionId: roles['Key Vault Secrets User']
    principalType: 'ServicePrincipal'
  }
}

module keyVaultContainerAppRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'keyVaultContainerAppRoleServicePrincipal'
  params: {
    principalId: containerAppUserAssignedManagedIdentity.outputs.userAssignedManagedIdentityPrincipalId
    roleDefinitionId: roles['Key Vault Secrets User']
    principalType: 'ServicePrincipal'
  }
}

module containerRegistryContainerAppRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rgshared
  name: 'containerRegistryContainerAppRoleServicePrincipal'
  params: {
    principalId: containerAppUserAssignedManagedIdentity.outputs.userAssignedManagedIdentityPrincipalId
    roleDefinitionId: roles['AcrPull']
    principalType: 'ServicePrincipal'
  }
}

output staticWebAppName string = staticWebApp.outputs.staticWebAppName
output staticWebAppHostName string = staticWebApp.outputs.staticWebAppHostName
