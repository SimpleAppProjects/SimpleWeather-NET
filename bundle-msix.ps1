#MSIX Bundle script

param (
    [string]$BundleDir = "AppPackages",
    [string]$ProjectName = "SimpleWeather.Windows",
    [string]$Version
)

if ([string]::IsNullOrWhiteSpace($BundleDir))
{
    throw "Directory is invalid"
}

if ([string]::IsNullOrWhiteSpace($ProjectName))
{
    throw "ProjectName is invalid"
}

$WorkingDir = pwd
$PackageDir = [System.IO.Path]::Combine($WorkingDir, $ProjectName, $BundleDir)

$MsixBundleDir = ""

if ([string]::IsNullOrWhiteSpace($Version))
{
    $filter = [string]::Format("{0}*_Test", $ProjectName)
    $MsixBundleDir = Get-ChildItem "$PackageDir" -Filter $filter -Directory | Sort -Descending | Select-Object -First 1

    # Check version string
    $Version = "$MsixBundleDir".Replace("${ProjectName}_", "").Replace("_Test", "")

    if ($Version -notmatch "[0-9]{1,5}.[0-9]{1,5}.[0-9]{1,5}.[0-9]{1,5}")
    {
        throw "Version does not match format x.x.x.x"
    }

    $MsixBundleDir = [System.IO.Path]::Combine($PackageDir, $MsixBundleDir)
}
else
{
    $filter = [string]::Format("{0}_{1}_Test", $ProjectName, $Version)
    $MsixBundleDir = Get-ChildItem "$PackageDir" -Filter $filter -Directory | Select-Object -First 1
    $MsixBundleDir = [System.IO.Path]::Combine($PackageDir, $MsixBundleDir)
}

if ([string]::IsNullOrWhiteSpace($MsixBundleDir))
{
    throw "Unable to find bundle dir"
}
else
{
    $msixcount = (Get-ChildItem $MsixBundleDir -Filter "*.msix" -File | Measure-Object).Count

    if ($msixcount -le 0)
    {
        throw "No .msix packages found"
    }

    $BundleName = "${ProjectName}_${Version}"
    $BundleFileName = "${BundleName}.msixbundle"

    $temp_dir = "${BundleName}_temp"
    $temp_dir = [System.IO.Path]::Combine($PackageDir, $temp_dir)

    $null = [System.IO.Directory]::CreateDirectory($temp_dir)

    Get-ChildItem $MsixBundleDir -Filter "*.msix" -File | Copy-Item -Destination $temp_dir

    # Log output
    Write-Host "Output Dir: ${PackageDir}"
    Write-Host "Version: ${Version}"
    Write-Host "Bundle File: ${BundleName}"

    # Create msixbundle
    $OutputBundleFile = [System.IO.Path]::Combine($temp_dir, $BundleFileName)
    MakeAppx bundle /bv $Version /d $temp_dir /p $OutputBundleFile

    # Remove .msix files
    Get-ChildItem $temp_dir -Filter "*.msix" -File | Remove-Item

    # Copy symbol files
    Get-ChildItem $MsixBundleDir -Filter "*.msixsym" -File | Copy-Item -Destination $temp_dir
    # Rename .msixsym -> .appxsym
    Get-ChildItem $temp_dir -Filter "*.msixsym" -File | Rename-Item -NewName {$_.name -replace ".msixsym",".appxsym"}

    # Zip files
    $ZipFile = [System.IO.Path]::Combine($temp_dir, "${BundleName}.zip")
    Compress-Archive -Path ("$temp_dir\*.msixbundle", "$temp_dir\*.appxsym") -DestinationPath $ZipFile

    # Move to final destination
    $OutputFile = [System.IO.Path]::Combine($PackageDir, "${BundleName}.msixupload")
    Move-Item $ZipFile -Destination $OutputFile

    # Delete temp dir
    Remove-Item -Recurse -Force $temp_dir

    Write-Host "Bundle file created: ${OutputFile}"
}