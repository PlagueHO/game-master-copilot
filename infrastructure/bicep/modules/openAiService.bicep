param location string
param openAiServiceName string
param logAnalyticsWorkspaceId string
param logAnalyticsWorkspaceName string

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

  resource openAiServiceDeploymentTextDavinci003 'deployments@2022-12-01' = {
    name: 'text-davinci-003'
    properties: {
      model: {
        format: 'OpenAI'
        name: 'text-davinci-003'
        version: '1'
      }
      scaleSettings: {
        scaleType: 'Standard'
      }
    }
  }

  resource openAiServiceDeploymentChatGPT 'deployments@2022-12-01' = {
    name: 'gpt-35-turbo'
    properties: {
      model: {
        format: 'OpenAI'
        name: 'gpt-35-turbo'
        version: '0301'
      }
      scaleSettings: {
        scaleType: 'Standard'
      }
    }
  }

  resource openAiServiceDeploymentTextEmbeddingsAda002 'deployments@2022-12-01' = {
    name: 'text-embedding-ada-002'
    properties: {
      model: {
        format: 'OpenAI'
        name: 'text-embedding-ada-002'
        version: '2'
      }
      scaleSettings: {
        scaleType: 'Standard'
      }
    }
  }
}

// Add the diagnostic settings to send logs and metrics to Log Analytics
resource openAiServiceDiagnosticSetting 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'send-to-${logAnalyticsWorkspaceName}'
  scope: openAiService
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
        category: 'RequestResponse'
        enabled: true
        retentionPolicy: {
          days: 0
          enabled: false 
        }
      }
      {
        category: 'Trace'
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

output openAiServiceEndpoint string = openAiService.properties.endpoint
