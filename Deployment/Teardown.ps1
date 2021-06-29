param(
    $StackName,
    $DeployResourceGroupName)

$resources = Get-AzResource -TagName "StackName" -TagValue $StackName -ResourceGroupName $DeployResourceGroupName
$resources | ForEach-Object {
    $id = $_.Id 
    Remove-AzResource -ResourceId $id -Force
    Write-Output "Removed $id"
}