param location string
param keyVaultName string
param resourceGroupName string
param logAnalyticsWorkspaceId string
param logAnalyticsWorkspaceName string
@secure()
param azureAdClientSecret string

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: subscription().tenantId
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enablePurgeProtection: true
    enableRbacAuthorization: true
  }

  resource keyVaultAzureAdClientSecret 'secrets@2023-02-01' = {
    name: 'AzureAd__ClientSecret'
    properties: {
      value: azureAdClientSecret
      contentType: 'text/plain'
    }
  }  
}

// var roles = {
//   'Key Vault Secrets User': '4633458b-17de-408a-b874-0445c86b69e6'
// }

// module keyVaultRoleSecretsUser 'roleAssignment.bicep' = {
//   name: 'keyVaultRoleSecretsUser'
//   params: {
//     principalId: reference(resourceId('Microsoft.Resources/deployments', deployment().name), '2022-09-01').identity.principalId
//     roleDefinitionId: roles['Key Vault Secrets User']
//     principalType: 'ServicePrincipal'
//   }
// }

// resource keyVaultAzureAdClientSecret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
//   name: 'AzureAd__ClientSecret'
//   parent: keyVault
//   dependsOn: [
//     keyVaultRoleSecretsUser
//   ]
//   properties: {
//     value: azureAdClientSecret
//     contentType: 'text/plain'
//   }
// }

// Add the diagnostic settings to send logs and metrics to Log Analytics
resource keyVaultDiagnosticSetting 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'send-to-${logAnalyticsWorkspaceName}'
  scope: keyVault
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'AuditEvent'
        enabled: true
        retentionPolicy: {
          days: 0
          enabled: false 
        }
      }
    ]
    metrics:[
      {
        category: 'AllMetrics'
        enabled: true
        retentionPolicy: {
          enabled: false
          days: 0
        }
      }
    ]
  }
}

output keyVaultId string = keyVault.id
output keyVaultName string = keyVault.name
