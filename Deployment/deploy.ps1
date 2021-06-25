param(
    $StackName, 
    $Location,
    $AppEnvironment,
    $Branch,
    $KeyVaultName,
    $DeployResourceGroupName,
    $KeyVaultResourceGroupName,
    $SubscriptionId,
    $Domain,
    $TenantId,
    $ClientId,
    $ClientSecret)

$found = $false
$allGroups = Get-AzResourceGroup -Location $Location
foreach ($group in $allGroups) {    
    if ($group.ResourceGroupName -eq $DeployResourceGroupName) {
        $found = $true
    }
}

if ($found -eq $false) {
    New-AzResourceGroup -Location $Location -Name $DeployResourceGroupName
}

$DeploymentName = (Get-Date -Format "yyyyMMddhhmmss") + "deployment"

$parameters = @{
    stackName         = $StackName;
    location          = $Location;
    appEnvironment    = $AppEnvironment;
    branch            = $Branch;
    keyVaultName      = $KeyVaultName;
    resourceGroupName = $KeyVaultResourceGroupName;
    subscriptionId    = $SubscriptionId;
    domain            = $Domain;
    tenantId          = $TenantId;
    clientId          = $ClientId;
    clientSecret      = $ClientSecret;
}

New-AzResourceGroupDeployment `
    -Name $DeploymentName `
    -ResourceGroupName $DeployResourceGroupName `
    -TemplateFile .\Deployment\deploy.bicep `
    -TemplateParameterObject $parameters 