param location string
param containerRegistryLoginServer string
param containerAppEnvironmentName string
param cosmosDbAccountName string
param buildVersion string

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbAccountName
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
  name: containerAppEnvironmentName
}

resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppEnvironmentName
  location: location
  identity: {
    type: 'SystemAssigned'
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
          identity: 'system'
          server: containerRegistryLoginServer
        }
      ]  
    }
    template: {
      containers: [
        {
          name: 'gmcopilot'
          image: '${containerRegistryLoginServer}/gmcopilot/gmcopilot:${buildVersion}'
          resources: {
            cpu: '0.25'
            memoty: '.5Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
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
              value: cosmosDbAccount.listConnectionStrings().connectionStrings[0].connectionString
            }
          ]
        }
      ]
    }
    scale: {
      minReplicas: 0
    }
  }
}

output containerAppUrl string = containerApp.properties.url
output containerAppIdentityPrincipalId string = containerApp.identity.principalId
