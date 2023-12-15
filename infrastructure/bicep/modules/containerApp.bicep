param location string
param containerRegistryLoginServer string
param userAssignedManagedIdentityName string
param containerAppEnvironmentName string
param cosmosDbAccountName string
param buildVersion string

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbAccountName
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
  name: containerAppEnvironmentName
}

resource userAssignedManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedManagedIdentityName
}

resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppEnvironmentName
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
      secrets: [
        {
          name: 'cosmosdb-connection-string'
          value: cosmosDbAccount.listConnectionStrings().connectionStrings[0].connectionString
        }
      ]
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
              secretRef: 'cosmosdb-connection-string'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 2
      }  
    }
  }
}

output containerAppUrl string = containerApp.properties.url
output containerAppIdentityPrincipalId string = containerApp.identity.principalId
