# Script to patch EasyRaffle.dll to remove authorization
# This requires dnSpy to be installed

$dllPath = "$env:APPDATA\XIVLauncher\installedPlugins\EasyRaffle\2.5.0.0\EasyRaffle.dll"
$backupPath = "$dllPath.backup"

Write-Host "Patching EasyRaffle.dll to remove authorization..."

# Backup the original
if (Test-Path $dllPath) {
    Copy-Item $dllPath -Destination $backupPath -Force
    Write-Host "Backed up original DLL to: $backupPath"
} else {
    Write-Host "ERROR: DLL not found at $dllPath"
    exit 1
}

Write-Host ""
Write-Host "To patch the DLL, you need to use dnSpy:"
Write-Host "1. Download dnSpy from: https://github.com/dnSpy/dnSpy/releases"
Write-Host "2. Open $dllPath in dnSpy"
Write-Host "3. Navigate to: EasyRaffle -> AuthorizationManager"
Write-Host "4. Edit the constructor to set IsAuthorizedUser = true immediately"
Write-Host "5. Edit ToggleMainUI in Plugin class to remove authorization check"
Write-Host "6. Save the module"
Write-Host ""
Write-Host "Or use the modified source code in this repository to rebuild the plugin."

