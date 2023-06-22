param location string
param cosmosDbAccountName string

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: cosmosDbAccountName
  location: location
  kind: 'GlobalDocumentDB'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
    }
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    isVirtualNetworkFilterEnabled: false
    enableAnalyticalStorage: false
    enableFreeTier: false
    publicNetworkAccess: 'Enabled'
  }
}


resource cosmosDbDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  name: 'dungeon-master-copilot'
  parent: cosmosDbAccount
  properties: {
    resource: {
      id: 'dungeon-master-copilot'
    }
    options: {
      autoscaleSettings: {
        maxThroughput: 1000
      }
    }
  }
}

resource cosmosDbTenantContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  name: 'tenants'
  parent: cosmosDbDatabase
  properties: {
    resource: {
      id: 'tenants'
      partitionKey: {
        paths: [
          '/TenantId'
        ]
        kind: 'Hash'
      }
    }
  }
}
resource cosmosDbCampaignContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  name: 'campaigns'
  parent: cosmosDbDatabase
  properties: {
    resource: {
      id: 'campaigns'
      partitionKey: {
        paths: [
          '/TenantId'
          '/CampaignId'
        ]
        kind: 'Hash'
      }
    }
  }
}

resource cosmosDbCharactersContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  name: 'characters'
  parent: cosmosDbDatabase
  properties: {
    resource: {
      id: 'characters'
      partitionKey: {
        paths: [
          '/TenantId'
          '/CampaignId'
          '/CharacterId'
        ]
        kind: 'Hash'
      }
    }
  }
}

output cosmosDbAccountName string = cosmosDbAccount.name
output cosmosDbAccountId string = cosmosDbAccount.id
