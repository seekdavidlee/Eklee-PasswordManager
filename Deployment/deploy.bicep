param location string = resourceGroup().location
param stackName string
param appEnvironment string
param branch string
param keyVaultName string
param resourceGroupName string
param subscriptionId string
param domain string
param tenantId string
param clientId string
param clientSecret string

resource hostingPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: stackName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
  }
}

resource app1 'Microsoft.Web/sites@2020-12-01' = {
  name: stackName
  location: location
  tags: {
    'stack-name': stackName
    'environment': appEnvironment
    'branch': branch
  }
  kind: 'app'
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: true
    siteConfig: {
      appSettings: [
        {
          'name': 'KeyVaultName'
          'value': keyVaultName
        }
        {
          'name': 'KeyVaultApi:BaseUrl'
          'value': 'https://${keyVaultName}.${environment().suffixes.keyvaultDns}'
        }
        {
          'name': 'KeyVaultApi:Scopes'
          'value': 'https://vault.azure.net/user_impersonation'
        }
        {
          'name': 'ManagementApi:BaseUrl'
          'value': 'https://management.azure.com'
        }
        {
          'name': 'ManagementApi:Scopes'
          'value': 'https://management.azure.com/user_impersonation'
        }
        {
          'name': 'Management:SubscriptionId'
          'value': subscriptionId
        }
        {
          'name': 'Management:ResourceGroupName'
          'value': resourceGroupName
        }
        {
          'name': 'AzureAd:Instance'
          'value': 'https://login.microsoftonline.com/'
        }
        {
          'name': 'AzureAd:Domain'
          'value': domain
        }
        {
          'name': 'AzureAd:TenantId'
          'value': tenantId
        }
        {
          'name': 'AzureAd:ClientId'
          'value': clientId
        }
        {
          'name': 'AzureAd:ClientSecret'
          'value': clientSecret
        }
        {
          'name': 'AzureAd:CallbackPath'
          'value': '/signin-oidc'
        }
      ]
    }
  }
}
