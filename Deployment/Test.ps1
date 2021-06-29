param($TenantId, $StackName, $TestConfig)

#$testConfigObj = Get-Content $TestConfig -Raw | ConvertFrom-Json -Depth 10
$testConfigObj = $TestConfig
$testConfigObj.ApplicationUrl = "https://$StackName.azurewebsites.net"
Write-Output ("::debug::Application Url " + $testConfigObj.ApplicationUrl)
$testConfigObj.TenantId = $TenantId
$testConfigString = $testConfigObj | ConvertTo-Json -Depth 10
Set-Content -Path Eklee.PasswordManager.Tests/appsettings.json -Value $testConfigString
dotnet test