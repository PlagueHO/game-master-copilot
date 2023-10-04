param location string
param containerAppEnvironmentName string
param logAnalyticsWorkspaceId string
param logAnalyticsWorkspaceName string

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: containerAppEnvironmentName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'azure-monitor'
    }
  }
}

// Add the diagnostic settings to send logs and metrics to Log Analytics
resource openAiServiceDiagnosticSetting 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'send-to-${logAnalyticsWorkspaceName}'
  scope: containerAppEnvironmentName
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'Audit'
        enabled: true
        retentionPolicy: {
          days: 0
          enabled: false 
        }
      }
      {
        category: 'ContainerAppConsoleLogs'
        enabled: true
        retentionPolicy: {
          days: 0
          enabled: false 
        }
      }
      {
        category: 'ContainerAppSystemLogs'
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

output containerAppEnvironmentName string = containerAppEnvironment.name
output containerAppEnvironmentId string = containerAppEnvironment.id
