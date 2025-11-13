# Immediate Fix for Authorization Error

The plugin is installed but still has the old authorization code in the DLL. Here's the **fastest way to fix it**:

## Quick Fix Using dnSpy (5 minutes)

1. **Download dnSpy:**
   - Go to: https://github.com/dnSpy/dnSpy/releases/latest
   - Download `dnSpy-net-win64.zip`
   - Extract it anywhere

2. **Open the DLL:**
   - Run `dnSpy.exe`
   - File → Open
   - Navigate to: `C:\Users\bigbr\AppData\Roaming\XIVLauncher\installedPlugins\EasyRaffle\2.5.0.0\EasyRaffle.dll`
   - Click Open

3. **Patch AuthorizationManager:**
   - In the left panel, expand: `EasyRaffle` → `AuthorizationManager`
   - Right-click on `AuthorizationManager` → **Edit Class (C#)**
   - Find the constructor (the method with the same name as the class)
   - Replace the entire constructor with:
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
   - Click **Compile**

4. **Patch OnFrameworkUpdate method:**
   - Still in AuthorizationManager, find `OnFrameworkUpdate`
   - Right-click → **Edit Method (C#)**
   - Replace with:
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
   - Click **Compile**

5. **Patch OnLogin method:**
   - Find `OnLogin` method
   - Right-click → **Edit Method (C#)**
   - Replace with:
   ```csharp
   private void OnLogin()
   {
       IsAuthorizedUser = true;
       initialized = true;
   }
   ```
   - Click **Compile**

6. **Patch Plugin.ToggleMainUI:**
   - Navigate to: `EasyRaffle` → `Plugin`
   - Find `ToggleMainUI` method
   - Right-click → **Edit Method (C#)**
   - Replace with:
   ```csharp
   public void ToggleMainUI()
   {
       ((Window)MainWindow).Toggle();
   }
   ```
   - Click **Compile**

7. **Save:**
   - File → **Save Module**
   - Click **OK** (it will save to the same location)

8. **Restart FFXIV/Dalamud** and try `/raffler` again!

The plugin should now work without authorization errors.

