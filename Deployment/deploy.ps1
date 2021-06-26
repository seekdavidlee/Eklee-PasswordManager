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

New-AzResourceGroupDeployment `
    -Name $DeploymentName `
    -ResourceGroupName $DeployResourceGroupName `
    -TemplateFile .\Deployment\deploy.json `
    -TemplateParameterObject $parameters

dotnet publish --configuration Release -o app
Compress-Archive -Path "/app/*" -DestinationPath app.zip
Publish-AzWebapp -ResourceGroupName $DeployResourceGroupName -Name $StackName -ArchivePath app.zip