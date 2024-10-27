$WorkingDir = pwd
$Condition = "${WorkingDir}\*SimpleWeather.Windows.exe*"

Get-Process | Where-Object {$_.Path -like $Condition} | Stop-Process -Force
Get-Process -Name backgroundTaskHost | Where-Object {$_.Modules -like "*Debug*SimpleWeather*"} | Stop-Process -Force