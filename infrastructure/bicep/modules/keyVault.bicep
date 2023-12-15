param location string
param keyVaultName string
param logAnalyticsWorkspaceId string
param logAnalyticsWorkspaceName string
@secure()
param entraIdClientSecret string

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
}

resource keyVaultEntraIdClientSecret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  name: 'EntraIdClientSecret'
  parent: keyVault
  properties: {
    value: entraIdClientSecret
    contentType: 'text/plain'
  }
}  

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
output keyVaultEntraIdClientSecretUri string = keyVaultEntraIdClientSecret.properties.secretUri
