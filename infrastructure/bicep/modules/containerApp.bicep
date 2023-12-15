param location string
param containerAppName string
param containerRegistryLoginServer string
param userAssignedManagedIdentityName string
param containerAppEnvironmentName string
param buildVersion string
param keyVaultName string
param cosmosDbAccountName string
param openAiServiceName string
param appConfigurationName string
param appInsightsConnectionString string
param azureOpenAiConfiguration array
param entraIdIssuerUrl string
param entraIdTenantId string
param entraIdClientId string
@secure()
param entraIdClientSecret string

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
  name: containerAppEnvironmentName
}

resource userAssignedManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedManagedIdentityName
}

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbAccountName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}

resource openAiService 'Microsoft.CognitiveServices/accounts@2023-05-01' existing = {
  name: openAiServiceName
}

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: appConfigurationName
}

var secrets = [
  {
    name: 'datastore-cosmosdb-connectionstring'
    value: cosmosDbAccount.listConnectionStrings().connectionStrings[0].connectionString
  }
  {
    name: 'applicationinsights-connectionstring'
    value: appInsightsConnectionString
  }
  {
    name: 'semantickernel-azureopenaiapikey'
    value: openAiService.listKeys().key1
  }
  {
    name: 'authorization-entraid-clientsecret'
    value: entraIdClientSecret
  }
]

var basicConfiguration = [
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
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: 'Development'
  }
  {
    name: 'Authorization__Type'
    value: 'EntraId'
  }
  {
    name: 'Authorization__EntraId__IssurerUrl'
    value: entraIdIssuerUrl
  }
  {
    name: 'Authorization__EntraId__TenantId'
    value: entraIdTenantId
  }
  {
    name: 'Authorization__EntraId__ClientId'
    value: entraIdClientId
  }
  {
    name: 'Authorization__EntraId__ClientSecret'
    secretRef: 'authorization-entraid-clientsecret'
  }
  {
    name: 'ApplicationInsights__ConnectionString'
    value: 'applicationinsights-connectionstring'
  }
  {
    name: 'SemanticKernel__PluginsDirectory'
    value: 'Plugins'
  }
  {
    name: 'SemanticKernel__AzureOpenAiApiKey'
    secretRef: 'semantickernel-azureopenaiapikey'
  }
  {
    name: 'DataStore__Type'
    value: 'CosmosDb'
  }
  {
    name: 'DataStore__CosmosDb__EndpointUri'
    value: cosmosDbAccount.properties.documentEndpoint
  }
  {
    name: 'DataStore__CosmosDb__Database'
    value: 'gmcopilot'
  }
  {
    name: 'DataStore__CosmosDb__ConnectionString'
    secretRef: 'datastore-cosmosdb-connectionstring'
  }
  {
    name: 'AppConfiguration__Endpoint'
    value: appConfiguration.properties.endpoint
  }
]

var environmentVariables = union(basicConfiguration, azureOpenAiConfiguration)

resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedManagedIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
        allowInsecure: false
        stickySessions: {
          affinity: 'sticky'
        }
      }
      registries: [
        {
          identity: userAssignedManagedIdentity.id
          server: containerRegistryLoginServer
        }
      ]
      secrets: secrets
    }
    template: {
      containers: [
        {
          name: 'gmcopilot'
          image: '${containerRegistryLoginServer}/gmcopilot/gmcopilot:${buildVersion}'
          probes: [
            {
              type: 'Liveness'
              httpGet: {
                path: '/health'
                port: 8080
              }
              initialDelaySeconds: 3
              periodSeconds: 3
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: environmentVariables
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 2
      }  
    }
  }
}

output applicationUrl string = containerApp.properties.latestRevisionFqdn
