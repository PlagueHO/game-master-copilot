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
param entraIdIssuerURL string = environment().authentication.loginEndpoint

@description('The Entra ID tenant ID to use for authentication.')
@secure()
param entraIdTenantId string

@description('The Entra ID client ID to use for authentication.')
@secure()
param entraIdClientId string

@description('The Entra ID client secret to use for authentication.')
@secure()
param entraIdClientSecret string

var logAnalyticsWorkspaceName = '${baseResourceName}-law'
var applicationInsightsName = '${baseResourceName}-ai'
var keyVaultName = '${baseResourceName}-akv'
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

var openAiConfigration = [
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
    entraIdClientSecret: entraIdClientSecret
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
module containerApp './modules/containerApp.bicep' = {
  name: 'containerApp'
  scope: rg
  params: {
    location: location
    containerAppName: containerAppName
    containerRegistryLoginServer: containerRegistryLoginServer
    userAssignedManagedIdentityName: containerAppUserAssignedManagedIdentityName
    containerAppEnvironmentName: containerAppEnvironmentName
    buildVersion: buildVersion
    keyVaultName: keyVault.outputs.keyVaultName
    cosmosDbAccountName: cosmosDbAccount.outputs.cosmosDbAccountName
    openAiServiceName: openAiService.outputs.openAiServiceName
    appConfigurationName: appConfiguration.outputs.appConfigurationName
    appInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    azureOpenAiConfiguration: openAiConfigration
    entraIdIssuerUrl: entraIdIssuerURL
    entraIdTenantId: entraIdTenantId
    entraIdClientId: entraIdClientId
  }
}

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

output applicationUrl string = containerApp.outputs.applicationUrl
output openAiServiceEndpoint string = openAiService.outputs.openAiServiceEndpoint
