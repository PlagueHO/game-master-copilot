param entraIdCiamName string

// It is not currently possible to create a CIAM directory using Bicep/ARM templates.
// This is a placeholder for future functionality.
resource entraIdCiam 'Microsoft.AzureActiveDirectory/ciamDirectories@2021-07-01-preview' = {
  name: entraIdCiamName
  location: 'United States'
}

output entraIdCiamId string = entraIdCiam.id
