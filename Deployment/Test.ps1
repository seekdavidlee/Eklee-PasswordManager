param($TenantId, $StackName, $ReportDirectory, $TestConfig)

$testConfigObj = $TestConfig | ConvertFrom-Json
$testConfigObj.ApplicationUrl = "https://$StackName.azurewebsites.net"
$testConfigObj.TenantId = $TenantId
$testConfigString = $testConfigObj | ConvertTo-Json -Depth 10
Set-Content -Path Eklee.PasswordManager.Tests/appsettings.json -Value $testConfigString
dotnet test --logger "junit;LogFilePath=$ReportDirectory/TestResults/results.xml"