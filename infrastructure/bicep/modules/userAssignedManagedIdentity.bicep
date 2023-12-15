param location string
param userAssignedManagedIdentityName string

resource userAssignedManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: userAssignedManagedIdentityName
  location: location 
}

output userAssignedManagedIdentityPrincipalId string = userAssignedManagedIdentity.properties.principalId
