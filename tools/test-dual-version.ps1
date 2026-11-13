param(
    [Parameter(Mandatory = $true)][string]$OldGameDataDir,
    [Parameter(Mandatory = $true)][string]$Sts2Dir,
    [Parameter(Mandatory = $true)][string]$RitsuDir,
    [string]$BaselineModDll = ''
)

$ErrorActionPreference = 'Stop'
$oldData = (Resolve-Path -LiteralPath $OldGameDataDir).Path
$gameDir = (Resolve-Path -LiteralPath $Sts2Dir).Path
$ritsu = (Resolve-Path -LiteralPath $RitsuDir).Path
$nativeData = Join-Path $gameDir 'data_sts2_windows_x86_64'
$repo = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$output = Join-Path $repo '.godot/dual-version-validation'
$noDeploy = Join-Path $PSScriptRoot 'DualVersionTests/NoDeploy.targets'
foreach ($path in @("$oldData/sts2.dll", "$nativeData/sts2.dll", "$ritsu/compat/0.111.0/STS2-RitsuLib.Runtime.dll", "$ritsu/compat/0.113.0/STS2-RitsuLib.Runtime.dll")) {
    if (-not (Test-Path -LiteralPath $path)) { throw "Missing validation input: $path" }
}
if ($BaselineModDll) { $BaselineModDll = (Resolve-Path -LiteralPath $BaselineModDll).Path }
New-Item -ItemType Directory -Force -Path $output | Out-Null
dotnet build (Join-Path $repo 'Oshinoko.csproj') -c Release -t:Rebuild "-p:Sts2Dir=$gameDir" "-p:CustomAfterMicrosoftCommonTargets=$noDeploy" "-p:OutputPath=$output/mod/" -v:minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet publish (Join-Path $PSScriptRoot 'DualVersionTests/DualVersionTests.csproj') -c Release "-p:Sts2Dir=$gameDir" -o "$output/runner" -v:minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
$mod = Join-Path $output 'mod/Oshinoko.dll'
$hash = (Get-FileHash -LiteralPath $mod -Algorithm SHA256).Hash
$hosts = @(@('0.111.0', $oldData), @('0.113.0', $nativeData))
foreach ($hostCase in $hosts) {
    $version = $hostCase[0]
    $lines = & "$output/runner/DualVersionTests.exe" $mod $hostCase[1] $nativeData "$ritsu/compat/$version" "$ritsu/shared" $version $hash $BaselineModDll $oldData 2>&1
    $code = $LASTEXITCODE
    $lines | Tee-Object -FilePath "$output/$version.log"
    if ($code -ne 0) { exit $code }
}
if ((Get-FileHash -LiteralPath $mod -Algorithm SHA256).Hash -ne $hash) { throw 'Mod DLL changed during validation' }
Write-Output "BOTH HOSTS PASSED; SAME DLL SHA256=$hash"
