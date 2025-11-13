# Quick Fix for Authorization Error

The plugin is installed but showing authorization errors because we're using the old DLL. Here's the quickest way to fix it:

## Option 1: Manual DLL Patch (Quickest)

Since the authorization check is simple, you can temporarily work around it by:

1. **Find your installed plugin:**
   - Navigate to: `%APPDATA%\XIVLauncher\installedPlugins\EasyRaffle\`
   - Or: `%LOCALAPPDATA%\XIVLauncher\addon\Hooks\dev\plugins\EasyRaffle\`

2. **Backup the DLL:**
   - Copy `EasyRaffle.dll` to `EasyRaffle.dll.backup`

3. **The issue:** The DLL still has the old authorization code. You need to rebuild it with the modified source code.

## Option 2: Build with Dalamud SDK

You need to set up the Dalamud plugin development environment:

1. **Install Dalamud Plugin Template:**
   - Follow: https://github.com/goatcorp/DalamudPlugins
   - Or use the Dalamud plugin template in Visual Studio

2. **Copy the modified source code** from this repository

3. **Build the plugin** - it will create a new DLL without authorization

4. **Replace the DLL** in the installed plugin folder

## Option 3: Use dnSpy to Patch (Advanced)

1. Download dnSpy: https://github.com/dnSpy/dnSpy/releases
2. Open `EasyRaffle.dll` in dnSpy
3. Navigate to `EasyRaffle.AuthorizationManager`
4. Edit the `OnFrameworkUpdate` method to always set `IsAuthorizedUser = true`
5. Save the modified DLL

## Temporary Workaround

Until we can rebuild, you could:
- Manually edit the DLL using a hex editor (not recommended)
- Or wait for a proper rebuild with Dalamud SDK

The source code is already fixed - we just need to compile it with the Dalamud SDK references.

