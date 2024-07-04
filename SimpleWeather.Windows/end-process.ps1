$WorkingDir = pwd
$Condition = "${WorkingDir}\*SimpleWeather.Windows.exe*"

Get-Process | Where-Object {$_.Path -like $Condition} | Stop-Process -Force