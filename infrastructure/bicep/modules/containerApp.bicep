@description('The Azure region to deploy the container app into')
param location string

@description('The name of the Container App to deploy')
param containerAppName string

@description('The URL of the container registry containing the image to deploy')
param containerRegistryLoginServer string

@description('The name of the user assigned managed identity to use for the container app')
param userAssignedManagedIdentityName string

@description('The name of the container app environment to deploy into')
param containerAppEnvironmentName string

@description('The containers to deploy')
param containers array

@description('An array of secrets to make available to the container app')
param secrets array

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
  name: containerAppEnvironmentName
}

resource userAssignedManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedManagedIdentityName
}

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
      containers: containers
      scale: {
        minReplicas: 0
        maxReplicas: 2
      }  
    }
  }
}

output applicationUrl string = containerApp.properties.latestRevisionFqdn
