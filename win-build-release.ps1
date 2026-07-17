param(
    [string]$BundleDir = "AppPackages",
    [string]$ProjectName = "SimpleWeather.Windows",
    [string]$ManifestPath = 'Package.appxmanifest',
    [string]$TargetFramework = 'net10.0-windows10.0.22621.0',
    [bool]$Build = $True,
    [bool]$Clean = $True
)

if ([string]::IsNullOrWhiteSpace($BundleDir))
{
    throw "Directory is invalid"
}

if ([string]::IsNullOrWhiteSpace($ProjectName))
{
    throw "ProjectName is invalid"
}

if (-not (Test-Path "${ProjectName}\${ManifestPath}")) {
    Write-Error "Manifest not found: $ManifestPath"
    exit 1
}

if (-not (Test-Path "${ProjectName}\${BundleDir}")) {
    New-Item -ItemType Directory "${ProjectName}\${BundleDir}"
}

[xml]$xml = Get-Content -Path "${ProjectName}\${ManifestPath}" -Raw

# Use local-name() in XPath to avoid namespace issues
$versionAttr = $xml.SelectSingleNode("/*[local-name() = 'Package']/*[local-name() = 'Identity']/@Version")
$Version = $versionAttr.Value

if ($Version) {
    Write-Host "Version: $Version"
} else {
    Write-Error "Version attribute not found in manifest."
    exit 1
}

# Build bundles

$WorkingDir = pwd
$PackageDir = [System.IO.Path]::Combine($WorkingDir, $ProjectName, $BundleDir)

if ($Build) {
    if ($Clean) {
        .\win-clean.bat
    }

    cd $ProjectName

    msbuild /r /t:Restore /p:Configuration=Release /p:PublishReadyToRun=true

    msbuild /p:Configuration=Release /p:Platform=x86 /p:RuntimeIdentifier=win-x86 /p:GenerateAppxPackageOnBuild=true /p:AppxBundle=Never /p:UapAppxPackageBuildMode=SideloadOnly  /p:GenerateSatelliteAssembliesForCore=true

    msbuild /p:Configuration=Release /p:Platform=x64 /p:RuntimeIdentifier=win-x64 /p:GenerateAppxPackageOnBuild=true /p:AppxBundle=Never /p:UapAppxPackageBuildMode=SideloadOnly /p:GenerateSatelliteAssembliesForCore=true

    msbuild /p:Configuration=Release /p:Platform=ARM64 /p:RuntimeIdentifier=win-arm64 /p:GenerateAppxPackageOnBuild=true /p:AppxBundle=Never /p:UapAppxPackageBuildMode=SideloadOnly /p:GenerateSatelliteAssembliesForCore=true
}

cd $ProjectName

# Verify builds completed successfully
$msixcount = (Get-ChildItem $PackageDir -Filter "${ProjectName}_${Version}_*_Test" -Directory | Measure-Object).Count

if ($msixcount -le 0)
{
    throw "No .msixupload packages found"
}

# Create temp bundle dir
$BundleName = "${ProjectName}_${Version}"
$BundleFileName = "${BundleName}.msixbundle"

$temp_dir = "${BundleName}_temp"
$temp_dir = [System.IO.Path]::Combine($PackageDir, $temp_dir)

$null = [System.IO.Directory]::CreateDirectory($temp_dir)

# Copy msix files
Write-Host "PackageDir: $PackageDir"
Write-Host "temp_dir: $temp_dir"
Get-ChildItem $PackageDir -Filter "${ProjectName}_${Version}_*_Test" -Directory | Get-ChildItem -Filter "${ProjectName}_${Version}_*.msix" -File | Copy-Item -Destination $temp_dir

# Log output
Write-Host "Output Dir: ${PackageDir}"
Write-Host "Version: ${Version}"
Write-Host "Bundle File: ${BundleName}"

# Create msixbundle
$OutputBundleFile = [System.IO.Path]::Combine($temp_dir, $BundleFileName)
MakeAppx bundle  /d $temp_dir /p $OutputBundleFile

# Remove .msix files
Get-ChildItem $temp_dir -Filter "*.msix" -File | Remove-Item

# Copy symbol files
Get-ChildItem $PackageDir -Filter "${ProjectName}_${Version}_*_Test" -Directory | Get-ChildItem -Filter "${ProjectName}_${Version}_*.appxsym" -File | Copy-Item -Destination $temp_dir

# Zip files
$ZipFile = [System.IO.Path]::Combine($temp_dir, "${BundleName}.zip")
Compress-Archive -Path ("$temp_dir\*.msixbundle", "$temp_dir\*.appxsym") -DestinationPath $ZipFile

# Move to final destination
$OutputFile = [System.IO.Path]::Combine($PackageDir, "${BundleName}.msixupload")
Move-Item $ZipFile -Destination $OutputFile

# Delete temp dir
Remove-Item -Recurse -Force $temp_dir

Write-Host "Bundle file created: ${OutputFile}"

cd $WorkingDir