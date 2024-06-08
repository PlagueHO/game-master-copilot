provider microsoftGraph

// This can't be used yet, as the Microsoft Graph provider doesn't allow deploying
// to an Entra External ID CIAM tenant that does not have an Azure subscription.
// But we'll store this 
resource clientApplication 'Microsoft.Graph/applications@v1.0' = {
  uniqueName: 'game-master-copilot-client'
  displayName: 'game-master-copilot-client'
  id: '996fbc46-fa66-4d29-ac35-492eb70975ca'
  appId: '7ff3f349-5bf4-48c7-8ea7-6e5536d0ae51'
  signInAudience: 'AzureADMyOrg'
  publisherDomain: 'gmcopilot.onmicrosoft.com'
  identifierUris: [
    'api://b1a02918-c873-4c7f-ba43-248171c138fe'
  ]
  replyUrlsWithType: [
    {
        url: 'https://delightful-desert-035df1d10.4.azurestaticapps.net/authentication/login-callback'
        type: 'Spa'
    }
    {
        url: 'https://localhost:7217/authentication/login-callback'
        type: 'Spa'
    }
  ]
  requiredResourceAccess: [
    {
      resourceAppId: 'b1a02918-c873-4c7f-ba43-248171c138fe'
      resourceAccess: [
        {
            id: 'c1c32cf0-1329-4662-bb0f-cdfc06807bf8'
            type: 'Scope'
        }
        {
            id: 'a60075de-c430-41df-b7d6-9c98540bc6b6'
            type: 'Scope'
        }
      ]
    }
    {
      resourceAppId: '00000003-0000-0000-c000-000000000000'
      resourceAccess: [
        {
            id: '64a6cdd6-aab1-4aaf-94b8-3cc8405e90d0'
            type: 'Scope'
        }
        {
            id: '7427e0e9-2fba-42fe-b0c0-848c9e6a8182'
            type: 'Scope'
        }
        {
            id: '37f7f235-527c-4136-accd-4a02d197296e'
            type: 'Scope'
        }
        {
            id: '14dad69e-099b-42c9-810b-d002981feec1'
            type: 'Scope'
        }
        {
            id: 'e1fe6dd8-ba31-4d61-89e7-88639da4683d'
            type: 'Scope'
        }
      ]
    }
  ]
}

resource clientServicePrincipal 'Microsoft.Graph/servicePrincipals@v1.0' = {
  appId: clientApplication.appId
}

resource apiApplication 'Microsoft.Graph/applications@v1.0' = {
  uniqueName: 'game-master-copilot-api'
  displayName: 'game-master-copilot-api'
  id: '8bd06f81-dde9-4628-97f0-3ea81505f331'
  signInAudience: 'AzureADMyOrg'
  publisherDomain: 'gmcopilot.onmicrosoft.com'
  identifierUris: [
    'api://b1a02918-c873-4c7f-ba43-248171c138fe'
  ]
  requiredResourceAccess: [
    {
      resourceAppId: '00000003-0000-0000-c000-000000000000'
      resourceAccess: [
        {
            id: 'e1fe6dd8-ba31-4d61-89e7-88639da4683d'
            type: 'Scope'
        }
      ]
    }
  ]
  appRoles: [
    {
      allowedMemberTypes: [
        'User'
        'Application'
      ]
      description: 'Administrator Game Master Copilot'
      displayName: 'GMCopilot Administrator '
      id: '1b4f816e-5eaf-48b9-8613-7923830595ad'
      value: 'GMCopilot.Administrator'
      origin: 'Application'
      isEnabled: true
    }
    {
      allowedMemberTypes: [
        'User'
        'Application'
      ]
      displayName: 'GMCopilot User'
      id: '1b4f816e-5eaf-48b9-8613-7923830595ae'
      value: 'GMCopilot.User'
      description: 'Access Game Master Copilot as a User'
      origin: 'Application'
      isEnabled: true
    }
  ]
  oauth2AllowIdTokenImplicitFlow: false
  oauth2AllowImplicitFlow: false
  oauth2Permissions: [
    {
        adminConsentDescription: 'Allow the app to read and write the user\'s Game Master Copilot data.'
        adminConsentDisplayName: 'Read and write Game Master Copilot data for the user'
        id: 'c1c32cf0-1329-4662-bb0f-cdfc06807bf8'
        isEnabled: true
        origin: 'Application'
        type: 'User'
        userConsentDescription: 'Allow the app to read and write your Game Master Copilot data.'
        userConsentDisplayName: 'Read and write your Game Master Copilot data'
        value: 'GMCopilot.ReadWrite'
    }
    {
        adminConsentDescription: 'Allow the app to read the user\'s Game Master Copilot data.'
        adminConsentDisplayName: 'Read Game Master Copilot data for the user'
        id: 'a60075de-c430-41df-b7d6-9c98540bc6b6'
        isEnabled: true
        origin: 'Application'
        type: 'User'
        userConsentDescription: 'Allow the app to read your Game Master Copilot data.'
        userConsentDisplayName: 'Read your Game Master Copilot data'
        value: 'GMCopilot.Read'
    }
  ]
  oauth2RequirePostResponse: false
}

resource apiServicePrincipal 'Microsoft.Graph/servicePrincipals@v1.0' = {
  appId: apiApplication.appId
}
