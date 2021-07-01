param(
    $StackName, 
    $Location,
    $AppEnvironment,
    $Branch,
    $KeyVaultName,
    $DeployResourceGroupName,
    $KeyVaultResourceGroupName,
    $Domain,
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

$DeploymentName = "passwordmanager-" + (Get-Date -Format "yyyyMMddhhmmss") + "-deployment"

$parameters = @{
    stackName         = $StackName;
    location          = $Location;
    appEnvironment    = $AppEnvironment;
    branch            = $Branch;
    keyVaultName      = $KeyVaultName;
    resourceGroupName = $KeyVaultResourceGroupName;
    domain            = $Domain;
    clientId          = $ClientId;
    clientSecret      = $ClientSecret;
}

$deployment = New-AzResourceGroupDeployment `
    -Name $DeploymentName `
    -ResourceGroupName $DeployResourceGroupName `
    -TemplateFile .\Deployment\deploy.json `
    -TemplateParameterObject $parameters

try {

    $assignments = Get-AzRoleAssignment -Scope $deployment.Outputs.storageId.Value
    $results = $assignments | Where-Object { $_.ObjectId -eq $deployment.Outputs.passwordManagerAppIdentityId.Value }

    if ($results.Count -eq 0) {
        New-AzRoleAssignment -ObjectId $deployment.Outputs.passwordManagerAppIdentityId.Value `
            -RoleDefinitionName 'Storage Blob Data Contributor' `
            -Scope $deployment.Outputs.storageId.Value
    }
    else {
        Write-Host "Role has been assigned."
    }
}
catch [Microsoft.Rest.Azure.CloudException] {
    Resolve-AzError $Error[0]
    throw "An error has occured on role assignment."
}

dotnet publish --configuration Release -o .\app
Compress-Archive -Path ".\app\*" -DestinationPath .\app.zip
Publish-AzWebapp -ResourceGroupName $DeployResourceGroupName -Name $StackName -ArchivePath .\app.zip -Force