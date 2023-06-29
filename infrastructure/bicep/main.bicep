targetScope = 'subscription'

@description('The location to deploy the main demo into.')
@allowed([
  'EastUS'
  'FranceCentral'
  'SouthCentralUS'
  'WestEurope'
  'UKSouth'
])
param location string = 'SouthCentralUS'

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

var openAiModelDeployments = [
  {
    name: 'text-davinci-003'
    modelName: 'text-davinci-003'
    version: '1'
    sku: 'Standard'
    capacity: 50
  }
  {
    name: 'gpt-35-turbo'
    modelName: 'gpt-35-turbo'
    version: '0301'
    sku: 'Standard'
    capacity: 50
  }
  {
    name: 'text-embedding-ada-002'
    modelName: 'text-embedding-ada-002'
    version: '2'
    sku: 'Standard'
    capacity: 50
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
    logAnalyticsWorkspaceName: '${baseResourceName}-law'
    applicationInsightsName: '${baseResourceName}-ai'
  }
}

module keyVault './modules/keyVault.bicep' = {
  name: 'keyVault'
  scope: rg
  params: {
    location: location
    keyVaultName: '${baseResourceName}-akv'
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: '${baseResourceName}-law'
    azureAdClientSecret: azureAdClientSecret
  }
}

module cosmosDbAccount './modules/cosmosDbAccount.bicep' = {
  name: 'cosmosDbAccount'
  scope: rg
  params: {
    location: location
    cosmosDbAccountName: '${baseResourceName}-cdb'
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
    openAiServiceName: '${baseResourceName}-oai'
    openAiModeldeployments: openAiModelDeployments
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: '${baseResourceName}-law'
  }
}

module cognitiveSearch './modules/cognitiveSearch.bicep' = {
  name: 'cognitiveSearch'
  scope: rg
  dependsOn: [
    monitoring
  ]
  params: {
    location: location
    cognitiveSearchName: '${baseResourceName}-cog'
    sku: 'basic'
    replicaCount: 1
    partitionCount: 1
    hostingMode: 'default'
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceName: '${baseResourceName}-law'
  }
}

module appServicePlan './modules/appServicePlan.bicep' = {
  name: 'appServicePlan'
  scope: rg
  params: {
    location: location
    appServicePlanName: '${baseResourceName}-asp'
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
    appInsightsInstrumentationKey: monitoring.outputs.applicationInsightsInstrumentationKey
    appInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    azureOpenAiEndpoint: openAiService.outputs.openAiServiceEndpoint
    azureOpenAiDeploymentText: openAiModelDeployments[0].name
    azureOpenAiDeploymentChat: openAiModelDeployments[1].name
    azureOpenAiDeploymentTextEmbedding: openAiModelDeployments[2].name
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
    storageAccountName: replace('${baseResourceName}data','-','')
  }
}

var roles = {
    'Cognitive Services OpenAI User': '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
    'Storage Blob Data Contributor': 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
    'Cosmos DB Account Reader Role': 'fbdf93bf-df7d-467e-a4d2-9458aa1360c8'
    'Key Vault Secrets User': '4633458b-17de-408a-b874-0445c86b69e6'
}

module openAiServiceRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'openAiServiceRoleServicePrincipal'
  params: {
    principalId: webAppBlazor.outputs.webAppIdentityPrincipalId
    roleDefinitionId: roles['Cognitive Services OpenAI User']
    principalType: 'ServicePrincipal'
  }
}

module cosmosDbRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'cosmosDbRoleServicePrincipal'
  params: {
    principalId: webAppBlazor.outputs.webAppIdentityPrincipalId
    roleDefinitionId: roles['Cosmos DB Account Reader Role']
    principalType: 'ServicePrincipal'
  }
}

module storageAccountRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'storageAccountRoleServicePrincipal'
  params: {
    principalId: webAppBlazor.outputs.webAppIdentityPrincipalId
    roleDefinitionId: roles['Storage Blob Data Contributor']
    principalType: 'ServicePrincipal'
  }
}

module keyVaultRoleServicePrincipal 'modules/roleAssignment.bicep' = {
  scope: rg
  name: 'keyVaultRoleServicePrincipal'
  params: {
    principalId: webAppBlazor.outputs.webAppIdentityPrincipalId
    roleDefinitionId: roles['Key Vault Secrets User']
    principalType: 'ServicePrincipal'
  }
}

output webAppName string = webAppBlazor.outputs.webAppName
output webAppHostName  string = webAppBlazor.outputs.webAppHostName
output webAppStagingName string = webAppBlazor.outputs.webAppStagingName
output webAppStagingHostName  string = webAppBlazor.outputs.webAppStagingHostName
output openAiServiceEndpoint string = openAiService.outputs.openAiServiceEndpoint
