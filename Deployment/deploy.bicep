param location string = resourceGroup().location
param stackName string
param appEnvironment string
param branch string
param keyVaultName string
param resourceGroupName string
param subscriptionId string = subscription().id
param domain string
param tenantId string = subscription().tenantId
param clientId string
param clientSecret string

var tags = {
  'stack-name': stackName
  'environment': appEnvironment
  'branch': branch
}

var appInsightsTag = {
  // circular dependency means we can't reference functionApp directly  /subscriptions/<subscriptionId>/resourceGroups/<rg-name>/providers/Microsoft.Web/sites/<appName>"
  'hidden-link:/subscriptions/${subscription().id}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Web/sites/${stackName}': 'Resource'
  'team': 'platform'
}

resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: stackName
  location: location
  kind: 'web'
  tags: union(tags, appInsightsTag)
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: stackName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
  }
  tags: tags
}

resource passwordManagerApp 'Microsoft.Web/sites@2020-12-01' = {
  name: stackName
  location: location
  tags: tags
  kind: 'app'
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: true
    siteConfig: {
      netFrameworkVersion: 'v5.0'
      webSocketsEnabled: true
      appSettings: [
        {
          'name': 'APPINSIGHTS_INSTRUMENTATIONKEY'
          'value': appInsights.properties.InstrumentationKey
        }
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
        {
          'name': 'ApplicationInsightsAgent_EXTENSION_VERSION'
          'value': '~2'
        }
        {
          'name': 'XDT_MicrosoftApplicationInsights_Mode'
          'value': 'default'
        }
        {
          'name': 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          'value': appInsights.properties.ConnectionString
        }
      ]
    }
  }
}
