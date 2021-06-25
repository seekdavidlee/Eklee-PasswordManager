# Introduction
This is a Blazor Server based app that connects to a backend Azure Key Vault. It also uses Azure SignalR.

## Build Status
![Build status](https://github.com/seekdavidlee/Eklee-PasswordManager/actions/workflows/app.yml/badge.svg)

# Setup
The following identity settings need to be configured before the project can be successfully executed. For more info see https://aka.ms/dotnet-template-ms-identity-platform. 

The Domain name would be your Azure Active Directory, usually in the form of [tenant name].onmicrosoft.com. The Tenant Id would also be found in your Azure Active Directory, in the form of a GUID. 

The Client Id and Client Secret would be part of your App Registration process. You can follow the process here to create your App Registration: https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app. As a further note when you create your App Registration, your Redirect Url would initially be http://localhost:[port number]. 

The Azure Key Vault name would be the name of your Azure Key Vault. For more information, pay attention to the Azure Key Vault Setup section below.

```
{
	"AzureAd": {
		"Instance": "https://login.microsoftonline.com/",
		"Domain": "",
		"TenantId": "",
		"ClientId": "",
		"ClientSecret": "",
		"CallbackPath": "/signin-oidc"
	},
	"KeyVaultName": "",
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft": "Warning",
			"Microsoft.Hosting.Lifetime": "Information"
		}
	},
	"AllowedHosts": "*",
	"KeyVaultApi": {
		"BaseUrl": "https://<Your Azure Key Vault Name>.vault.azure.net",
		"Scopes": "https://vault.azure.net/user_impersonation"
	},
	"ManagementApi": {
		"BaseUrl": "https://management.azure.com",
		"Scopes": "https://management.azure.com/user_impersonation"
	},
	"Management": {
		"SubscriptionId": "<Your Subscription Id where Azure Key Vault is hosted>",
		"ResourceGroupName": "<Your Resource Group hosting the Azure Key Vault instance>"
	}
}
```

Note that your user would need to be able to access the management API so the app can determine if a individual secret can be accessed.