param location string
param tenantName string
param tenantDisplayName string

resource b2cTenant 'Microsoft.AzureActiveDirectory/b2cDirectories@2023-01-18-preview' = {
  name: tenantName
  location: location
  sku: {
    name: 'PremiumP1'
    tier: 'A0'
  }
  properties: {
    createTenantProperties: {
      displayName: tenantDisplayName
    }
  }
}
