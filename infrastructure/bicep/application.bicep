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

@description('The name of the resource group that will contain all the resources.')
param resourceGroupName string

@description('The base name that will prefixed to all Azure resources deployed to ensure they are unique.')
param baseResourceName string

@description('The Azure App Service SKU for running the Blazor App.')
@allowed([
  'F1'
  'B1'
  'B2'
  'B3'
  'S1'
  'S2'
  'S3'
  'P1V2'
  'P2V2'
  'P3V2'
  'P0V3'
  'P1V3'
  'P2V3'
  'P3V3'
])
param appServicePlanConfiguration string = 'P0V3'

@description('The Azure AD instance to use for authentication.')
param azureAdInstance string = environment().authentication.loginEndpoint

@description('The Azure AD domain to use for authentication.')
param azureAdDomain string

@description('The Azure AD tenant ID to use for authentication.')
@secure()
param azureAdTenantId string

@description('The Azure AD client ID to use for authentication.')
@secure()
param azureAdClientId string

@description('The Azure AD client secret to use for authentication.')
@secure()
param azureAdClientSecret string

var logAnalyticsWorkspaceName = '${baseResourceName}-law'
var applicationInsightsName = '${baseResourceName}-ai'
var keyVaultName = '${baseResourceName}-akv'
var appConfigurationName = '${baseResourceName}-appconfig'
var appServiceName = '${baseResourceName}-asp'
var openAiServiceName = '${baseResourceName}-oai'
var aiSearchName = '${baseResourceName}-aisearch'
var cosmosDbAccountName = '${baseResourceName}-cdb'
var storageAccountName = replace('${baseResourceName}data','-','')
var containerAppEnvironmentName = '${baseResourceName}-cae'

var openAiModelDeployments = [
  {
    name: 'gpt-35-turbo'
    modelName: 'gpt-35-turbo'
    version: '1106'
    sku: 'Standard'
    capacity: 60
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
    capacity: 60
  }
]

var openAiWebConfigration = [
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
    value: true
  }
  {
    name: 'SemanticKernel__AzureOpenAiChatCompletionServices__0__AlsoAsTextCompletion'
    value: true
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
    value: false
  }
  {
    name: 'SemanticKernel__AzureOpenAiChatCompletionServices__1__AlsoAsTextCompletion'
    value: true
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
    value: true
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
    value: true
  }
]

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
    azureAdClientSecret: azureAdClientSecret
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

module appServicePlan './modules/appServicePlan.bicep' = {
  name: 'appServicePlan'
  scope: rg
  params: {
    location: location
    appServicePlanName: appServiceName
    appServicePlanConfiguration: appServicePlanConfiguration
  }
}

module webAppBlazor './modules/webAppBlazor.bicep' = {
  name: 'webAppBlazor'
  scope: rg
  dependsOn: [
    appServicePlan
    cosmosDbAccount
    openAiService
    monitoring
  ]
  params: {
    location: location
    appServicePlanId: appServicePlan.outputs.appServicePlanId
    webAppName: baseResourceName
    keyVaultName: keyVault.outputs.keyVaultName
    cosmosDbAccountName: cosmosDbAccount.outputs.cosmosDbAccountName
    openAiServiceName: openAiService.outputs.openAiServiceName
    appConfigurationName: appConfiguration.outputs.appConfigurationName
    appInsightsInstrumentationKey: monitoring.outputs.applicationInsightsInstrumentationKey
    appInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    azureOpenAiWebConfiguration: openAiWebConfigration
    azureAdInstance: azureAdInstance
    azureAdDomain: azureAdDomain
    azureAdTenantId: azureAdTenantId
    azureAdClientId: azureAdClientId
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

module containerApp './modules/containerApp.bicep' = {
  name: 'containerApp'
  scope: rg
  params: {
    location: location
    containerAppEnvironmentName: containerAppEnvironmentName
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
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

module openAiServiceWebAppRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'openAiServiceWebAppRoleServicePrincipal'
  params: {
    principalId: webAppBlazor.outputs.webAppIdentityPrincipalId
    roleDefinitionId: roles['Cognitive Services OpenAI User']
    principalType: 'ServicePrincipal'
  }
}

module cosmosDbWebAppRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'cosmosDbWebAppRoleServicePrincipal'
  params: {
    principalId: webAppBlazor.outputs.webAppIdentityPrincipalId
    roleDefinitionId: roles['Cosmos DB Account Reader Role']
    principalType: 'ServicePrincipal'
  }
}

module storageAccountWebAppRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'storageAccountWebAppRoleServicePrincipal'
  params: {
    principalId: webAppBlazor.outputs.webAppIdentityPrincipalId
    roleDefinitionId: roles['Storage Blob Data Contributor']
    principalType: 'ServicePrincipal'
  }
}

module keyVaultWebAppRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'keyVaultWebAppRoleServicePrincipal'
  params: {
    principalId: webAppBlazor.outputs.webAppIdentityPrincipalId
    roleDefinitionId: roles['Key Vault Secrets User']
    principalType: 'ServicePrincipal'
  }
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

module appConfigurationWebAppRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'appConfigurationWebAppRoleServicePrincipal'
  params: {
    principalId: webAppBlazor.outputs.webAppIdentityPrincipalId
    roleDefinitionId: roles['App Configuration Data Reader']
    principalType: 'ServicePrincipal'
  }
}

module containerRegistryAppConfigurationRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'containerRegistryAppConfigurationRoleServicePrincipal'
  params: {
    principalId: appConfiguration.outputs.appConfigurationIdentityPrincipalId
    roleDefinitionId: roles['AcrPull']
    principalType: 'ServicePrincipal'
  }
}

output webAppName string = webAppBlazor.outputs.webAppName
output webAppHostName  string = webAppBlazor.outputs.webAppHostName
output webAppStagingName string = webAppBlazor.outputs.webAppStagingName
output webAppStagingHostName  string = webAppBlazor.outputs.webAppStagingHostName
output openAiServiceEndpoint string = openAiService.outputs.openAiServiceEndpoint