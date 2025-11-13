# Fix Authorization Error - Quick Solution

The plugin is installed but showing authorization errors because the DLL still has the old authorization code. Here's how to fix it:

## Solution: Use dnSpy to Patch the DLL

1. **Download dnSpy:**
   - Go to: https://github.com/dnSpy/dnSpy/releases
   - Download and extract dnSpy

2. **Open the DLL:**
   - Open dnSpy
   - File → Open → Navigate to your installed plugin folder:
     - `%APPDATA%\XIVLauncher\installedPlugins\EasyRaffle\EasyRaffle.dll`
     - Or: `%LOCALAPPDATA%\XIVLauncher\addon\Hooks\dev\plugins\EasyRaffle\EasyRaffle.dll`

3. **Edit AuthorizationManager:**
   - In dnSpy, navigate to: `EasyRaffle` → `AuthorizationManager`
   - Right-click on `AuthorizationManager` → Edit Class (C#)
   - Replace the constructor with:
   ```csharp
   public AuthorizationManager(IFramework framework, IClientState clientState, IPluginLog log, IChatGui chatGui)
   {
       this.framework = framework;
       this.clientState = clientState;
       this.log = log;
       this.chatGui = chatGui;
       IsAuthorizedUser = true;
       initialized = true;
   }
   ```
   - Replace `OnFrameworkUpdate` with:
   ```csharp
   private void OnFrameworkUpdate(IFramework _)
   {
       if (!initialized && clientState.LocalPlayer != null)
       {
           IsAuthorizedUser = true;
           initialized = true;
           framework.Update -= new OnUpdateDelegate(OnFrameworkUpdate);
       }
   }
   ```
   - Replace `OnLogin` with:
   ```csharp
   private void OnLogin()
   {
       IsAuthorizedUser = true;
       initialized = true;
   }
   ```

4. **Edit Plugin.cs:**
   - Navigate to: `EasyRaffle` → `Plugin`
   - Find the `ToggleMainUI` method
   - Right-click → Edit Method (C#)
   - Replace with:
   ```csharp
   public void ToggleMainUI()
   {
       ((Window)MainWindow).Toggle();
   }
   ```

5. **Save:**
   - File → Save Module
   - Choose the DLL location and save

6. **Restart FFXIV/Dalamud** and the plugin should work!

## Alternative: Rebuild from Source

If you have Dalamud SDK set up, you can rebuild the plugin from the modified source code in this repository.

