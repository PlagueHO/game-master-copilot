param location string
param appServicePlanId string
param webAppName string
param keyVaultName string
param cosmosDbAccountName string
param appInsightsInstrumentationKey string
param appInsightsConnectionString string
param azureOpenAiEndpoint string
param azureOpenAiDeploymentText string
param azureOpenAiDeploymentChat string
param azureOpenAiDeploymentTextEmbedding string
param azureAdInstance string
param azureAdDomain string
@secure()
param azureAdTenantId string
@secure()
param azureAdClientId string

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbAccountName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}

resource keyVaultAzureAdClientSecret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' existing = {
  name: 'AzureAdClientSecret'
  parent: keyVault
}

var appSettings = [
  {
    name: 'AzureAd__Instance'
    value: azureAdInstance
  }
  {
    name: 'AzureAd__Domain'
    value: azureAdDomain
  }
  {
    name: 'AzureAd__TenantId'
    value: azureAdTenantId
  }
  {
    name: 'AzureAd__ClientId'
    value: azureAdClientId
  }
  {
    name: 'AzureAd__ClientSecret'
    value: '@Microsoft.KeyVault(SecretUri=${keyVaultAzureAdClientSecret.properties.secretUri})'
  }
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    value: appInsightsInstrumentationKey
  }
  {
    name: 'APPINSIGHTS_PROFILERFEATURE_VERSION'
    value: '1.0.0'
  }
  {
    name: 'APPINSIGHTS_SNAPSHOTFEATURE_VERSION'
    value: '1.0.0'
  }
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: appInsightsConnectionString
  }
  {
    name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
    value: '~2'
  }
  {
    name: 'DiagnosticServices_EXTENSION_VERSION'
    value: '~3'
  }
  {
    name: 'InstrumentationEngine_EXTENSION_VERSION'
    value: 'disabled'
  }
  {
    name: 'SnapshotDebugger_EXTENSION_VERSION'
    value: 'disabled'
  }
  {
    name: 'XDT_MicrosoftApplicationInsights_BaseExtensions'
    value: 'disabled'
  }
  {
    name: 'XDT_MicrosoftApplicationInsights_Mode'
    value: 'recommended'
  }
  {
    name: 'XDT_MicrosoftApplicationInsights_PreemptSdk'
    value: 'disabled'
  }
  {
    name: 'SemanticKernel__PluginsDirectory'
    value: 'Plugins'
  }
  {
    name: 'SemanticKernel__AzureOpenAiTextCompletionServices__0__Id'
    value: 'TextCompletion'
  }
  {
    name: 'SemanticKernel__AzureOpenAiTextCompletionServices__0__Endpoint'
    value: azureOpenAiEndpoint
  }
  {
    name: 'SemanticKernel__AzureOpenAiTextCompletionServices__0__Deployment'
    value: azureOpenAiDeploymentText
  }
  {
    name: 'SemanticKernel__AzureOpenAiTextCompletionServices__0__SetAsDefault'
    value: true
  }
  {
    name: 'SemanticKernel__AzureOpenAiChatCompletionServices__1__Id'
    value: 'ChatCompletion'
  }
  {
    name: 'SemanticKernel__AzureOpenAiChatCompletionServices__1__Endpoint'
    value: azureOpenAiEndpoint
  }
  {
    name: 'SemanticKernel__AzureOpenAiChatCompletionServices__1__Deployment'
    value: azureOpenAiDeploymentChat
  }
  {
    name: 'SemanticKernel__AzureOpenAiChatCompletionServices__1__SetAsDefault'
    value: true
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
    value: azureOpenAiEndpoint
  }
  {
    name: 'SemanticKernel__AzureOpenAiTextEmbeddingGenerationServices__2__Deployment'
    value: azureOpenAiDeploymentTextEmbedding
  }
  {
    name: 'SemanticKernel__AzureOpenAiTextEmbeddingGenerationServices__2__SetAsDefault'
    value: true
  }
  {
    name: 'CosmosDb__EndpointUri'
    value: cosmosDbAccount.properties.documentEndpoint
  }
  {
    name: 'CosmosDb__Database'
    value: 'dmcopilot'
  }
]

var connectionStrings = [
  {
    name: 'CosmosDb'
    connectionString: cosmosDbAccount.listConnectionStrings().connectionStrings[0].connectionString
    type: 'Custom'
  }
]

resource webApp 'Microsoft.Web/sites@2021-01-15' = {
  name: webAppName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    siteConfig: {
      numberOfWorkers: 1
      linuxFxVersion: 'DOTNETCORE|7.0'
      healthCheckPath: '/'
      appSettings: appSettings
      connectionStrings: connectionStrings
    }
    clientAffinityEnabled: true
  }

  resource config 'config@2021-01-15' = {
    name: 'web'
    properties: {
      httpLoggingEnabled: true
      logsDirectorySizeLimit: 35
      detailedErrorLoggingEnabled: true
      linuxFxVersion: 'DOTNETCORE|7.0'
      healthCheckPath: '/healthcheck'
    }
  }
}

resource WebAppStaging 'Microsoft.Web/sites/slots@2022-03-01' = {
  name: 'staging'
  parent: webApp
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    siteConfig: {
      numberOfWorkers: 1
      linuxFxVersion: 'DOTNETCORE|7.0'
      healthCheckPath: '/'
      appSettings: appSettings
      connectionStrings: connectionStrings
    }
    clientAffinityEnabled: true
  }

  resource config 'config@2021-01-15' = {
    name: 'web'
    properties: {
      httpLoggingEnabled: true
      logsDirectorySizeLimit: 35
      detailedErrorLoggingEnabled: true
      linuxFxVersion: 'DOTNETCORE|7.0'
      healthCheckPath: '/healthcheck'
    }
  }
}

output webAppName string = webApp.name
output webAppStagingName string = WebAppStaging.name
output webAppHostName  string = webApp.properties.defaultHostName
output webAppStagingHostName  string = WebAppStaging.properties.defaultHostName
output webAppIdentityPrincipalId string = webApp.identity.principalId
