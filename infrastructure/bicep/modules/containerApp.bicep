param location string
param containerAppEnvironmentName string
param logAnalyticsWorkspaceCustomerId string
param logAnalyticsWorkspaceSharedKey string

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: containerAppEnvironmentName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
          customerId: logAnalyticsWorkspaceCustomerId
          sharedKey: logAnalyticsWorkspaceSharedKey
      }
    }
  }
}

output containerAppEnvironmentName string = containerAppEnvironment.name
output containerAppEnvironmentId string = containerAppEnvironment.id
