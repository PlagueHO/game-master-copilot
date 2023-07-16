param location string
param appConfigurationName string
param logAnalyticsWorkspaceId string
param logAnalyticsWorkspaceName string

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: appConfigurationName
  location: location
  sku: {
    name: 'Free'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    disableLocalAuth: false
    publicNetworkAccess: 'Enabled'
    softDeleteRetentionInDays: 0
    enablePurgeProtection: false
  }
}

// Add the diagnostic settings to send logs and metrics to Log Analytics
resource appConfigurationDiagnosticSetting 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'send-to-${logAnalyticsWorkspaceName}'
  scope: appConfiguration
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'HttpRequest'
        enabled: true
        retentionPolicy: {
          days: 0
          enabled: false 
        }
      }
      {
        category: 'Audit'
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

output appConfigurationName string = appConfiguration.name
output appConfigurationId string = appConfiguration.id
output appConfigurationEndpoint string = appConfiguration.properties.endpoint
