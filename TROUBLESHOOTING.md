# Troubleshooting Installation Issues

## Common Issues and Solutions

### Issue: Plugin won't install / "Could not download" error

**Cause:** The `latest.zip` file doesn't exist yet or is corrupted.

**Solution:**
1. You MUST build the plugin first and create `latest.zip`
2. The plugin cannot be installed until `latest.zip` exists on GitHub

### Issue: "Repository not found" or "Invalid repository"

**Cause:** The repo.json URL might be incorrect or the file format is wrong.

**Solution:**
1. Verify the repo URL in Dalamud: `https://raw.githubusercontent.com/GamerLatte/Easy-Raffle/main/repo.json`
2. Make sure you're using the exact URL (case-sensitive)
3. Check that the repo.json file is valid JSON

### Issue: "Plugin manifest not found" or "Invalid manifest"

**Cause:** The EasyRaffle.json file might be missing from latest.zip or incorrectly formatted.

**Solution:**
1. Make sure `EasyRaffle.json` is included in `latest.zip`
2. Verify the manifest matches the repo.json structure
3. Check that InternalName matches: `EasyRaffle`

### Issue: "DLL not found" or "Assembly load error"

**Cause:** The EasyRaffle.dll is missing or dependencies are missing.

**Solution:**
1. Ensure `EasyRaffle.dll` is in `latest.zip`
2. Include `ECommons.dll` if it's a dependency
3. Make sure all required files are in the zip root (not in subfolders)

## Required Files in latest.zip

Your `latest.zip` MUST contain:
- `EasyRaffle.dll` (the compiled plugin)
- `EasyRaffle.json` (the plugin manifest)
- `easyraffle.ico` (icon file)
- `easyraffle.png` (optional, for display)
- `ECommons.dll` (if required as dependency)

## Step-by-Step Fix

1. **Build the plugin:**
   ```bash
   dotnet build --configuration Release
   ```

2. **Create release folder:**
   ```bash
   mkdir release
   ```

3. **Copy required files:**
   ```bash
   copy bin\Release\net9.0\EasyRaffle.dll release\
   copy EasyRaffle.json release\
   copy easyraffle.ico release\
   copy easyraffle.png release\
   # Copy ECommons.dll if needed
   ```

4. **Create zip:**
   ```bash
   Compress-Archive -Path release\* -DestinationPath latest.zip -Force
   ```

5. **Push to GitHub:**
   ```bash
   git add latest.zip
   git commit -m "Add latest.zip"
   git push
   ```

6. **Wait a few seconds** for GitHub to update the raw file URL

7. **Try installing again** in Dalamud

## Verifying Installation

After pushing `latest.zip`, verify it's accessible:
- Visit: https://raw.githubusercontent.com/GamerLatte/Easy-Raffle/main/latest.zip
- You should be able to download the file
- If you get a 404, the file isn't on GitHub yet

## Still Not Working?

1. Check Dalamud logs: `%AppData%\XIVLauncher\addon\Hooks\dev\`
2. Verify the repo.json URL is correct in Dalamud settings
3. Try removing and re-adding the repository
4. Make sure Dalamud is up to date

