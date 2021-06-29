param(
    $StackName,
    $DeployResourceGroupName)

$resources = Get-AzResource -TagName "stack-name" -TagValue $StackName -ResourceGroupName $DeployResourceGroupName

if ($resources.Length -eq 0) {
    Write-Output "::debug::No resource(s) with tag name 'stack-name' is found."
}

$resources | ForEach-Object {
    $id = $_.Id 
    Remove-AzResource -ResourceId $id -Force
    Write-Output "::debug::Removed resource $id"
}