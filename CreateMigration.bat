@echo off
set /p MigrationName="Enter name for migration: "

dotnet ef migrations add %MigrationName% --project EFProject --output-dir ..\SimpleWeather.EF\Data\Migrations\LocationDB --context LocationDBContext -v
dotnet ef migrations add %MigrationName% --project EFProject --output-dir ..\SimpleWeather.EF\Data\Migrations\WeatherDB --context WeatherDBContext -v

PAUSE