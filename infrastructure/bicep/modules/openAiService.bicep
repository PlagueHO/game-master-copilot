param location string
param openAiServiceName string
param openAiModeldeployments array

resource openAiService 'Microsoft.CognitiveServices/accounts@2022-12-01' = {
  name: openAiServiceName
  location: location
  sku: {
    name: 'S0'
  }
  kind: 'OpenAI'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
    customSubDomainName: openAiServiceName
  }
}

// Loop through the list of models and create a deployment for each

resource openAiServiceDeployment 'Microsoft.CognitiveServices/accounts/deployments@2022-12-01' = [for (model, i) in openAiModeldeployments: {
  parent: openAiService
  name: model.name
  properties: {
    model: {
      format: 'OpenAI'
      name: model.modelName
      version: model.modelVersion
    }
    scaleSettings: {
      scaleType: model.scaleType
    }
  }
}]

output openAiServiceEndpoint string = openAiService.properties.endpoint
