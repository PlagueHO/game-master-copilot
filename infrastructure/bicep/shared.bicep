targetScope = 'subscription'

@description('The location to deploy the Game Master Copilot shared resources into.')
@allowed([
  'AustraliaEast'
  'CanadaEast'
  'EastUS'
  'EastUS2'
  'FranceCentral'
  'JapanEast'
  'NorthCentralUS'
  'SouthCentralUS'
  'SwedenCentral'
  'SwitzerlandNorth'
  'WestEurope'
  'UKSouth'
])
param location string = 'CanadaEast'

@description('The name of the resource group that will contain the Game Master Copilot shared resources.')
param resourceGroupName string

@description('The base name that will prefixed to all Azure resources deployed to ensure they are unique.')
param baseResourceName string

var containerRegistryName = replace('${baseResourceName}acr','-','')

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

module containerRegistry './modules/containerRegistry.bicep' = {
  name: 'containerRegistry'
  scope: rg
  params: {
    location: location
    containerRegistryName: containerRegistryName
  }
}

output containerRegistryName string = containerRegistry.outputs.containerRegistryName
