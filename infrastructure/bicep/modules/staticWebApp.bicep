@allowed([
  'CentralUS'
  'EastUS2'
  'EastAsia'
  'WestEurope'
  'WestUS2'
])
param location string
param staticWebAppName string
@allowed([ 'Free', 'Standard' ])
param sku string = 'Free'

resource staticWebApp 'Microsoft.Web/staticSites@2022-09-01' = {
  name: staticWebAppName
  location: location
  sku: {
    name: sku
    size: sku
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

output appConfigurationName string = staticWebApp.name
output appConfigurationId string = staticWebApp.id
output appConfigurationEndpoint string = staticWebApp.properties.defaultHostname
