param location string
param tenantName string
param tenantDisplayName string
param countryCode string = 'US'

resource b2cTenant 'Microsoft.AzureActiveDirectory/b2cDirectories@2023-01-18-preview' = {
  name: tenantName
  location: location
  sku: {
    name: tenantName
    tier: 'A0'
  }
  properties: {
    createTenantProperties: {
      countryCode: countryCode
      displayName: tenantDisplayName
    }
  }
}
