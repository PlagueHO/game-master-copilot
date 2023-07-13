param location string
param containerAppEnvironmentName string
param logAnalyticsWorkspaceCustomerId string

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-04-01-preview' = {
  name: containerAppEnvironmentName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
          customerId: logAnalyticsWorkspaceCustomerId
      }
    }
  }
}

output containerAppEnvironmentName string = containerAppEnvironment.name
output containerAppEnvironmentId string = containerAppEnvironment.id
