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

@description('Enable Entra ID authentication')
param entraIdAuthentication bool = true

@description('If Authorization is enabled, the issuing URL of the Entra ID tenant to use')
param entraIdIssuerUrl string = environment().authentication.loginEndpoint

@description('If Authorization is enabled, the tenant ID of the Entra ID tenant to use')
param entraIdTenantId string

@description('If Authorization is enabled, the client ID of the application registration in the Entra ID tenant')
param entraIdClientId string

@description('If Authorization is enabled, the client secret of the application registration in the Entra ID tenant')
@secure()
param entraIdClientSecret string

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
  name: containerAppEnvironmentName
}

resource userAssignedManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedManagedIdentityName
}

// If authentication is being used, set the authorization-entraid-clientsecret and add it to the secrets array
var allSecrets = entraIdAuthentication ? union(secrets, [
    {
      name: 'authorization-entraid-clientsecret'
      value: entraIdClientSecret
    }
  ]
) : secrets

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
      secrets: allSecrets
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

// Only add authentication if entraIdClientId is set
resource containerAppAuth 'Microsoft.App/containerApps/authConfigs@2023-05-01' = if (entraIdClientId != null) {
  name: 'current'
  parent: containerApp
  properties: {
    globalValidation: {
      redirectToProvider: 'azureactivedirectory'
      unauthenticatedClientAction: 'RedirectToLoginPage'
    }
    identityProviders: {
      azureActiveDirectory: {
        isAutoProvisioned: false
        registration: {
          clientId: entraIdClientId
          clientSecretSettingName: 'authorization-entraid-clientsecret'
          openIdIssuer: '${entraIdIssuerUrl}/${entraIdTenantId}'
        }
        validation: {
          allowedAudiences: []
        }
      }
    }
    platform: {
      enabled: true
    }
  }
}

output applicationUrl string = containerApp.properties.latestRevisionFqdn
