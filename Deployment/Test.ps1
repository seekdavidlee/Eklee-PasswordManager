param($TenantId, $StackName, $TestConfig)

$testConfigObj = $TestConfig | ConvertFrom-Json
$testConfigObj.ApplicationUrl = "https://$StackName.azurewebsites.net"
$testConfigObj.TenantId = $TenantId
$testConfigString = $testConfigObj | ConvertTo-Json -Depth 10
Set-Content -Path Eklee.PasswordManager.Tests/appsettings.json -Value $testConfigString
dotnet test --logger trx --results-directory "TestResults"