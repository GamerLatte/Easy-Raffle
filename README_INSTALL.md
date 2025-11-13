# Installation Setup

## Current Status

The plugin repository is set up, but you need to build the plugin and create `latest.zip` before it can be installed via Dalamud.

## Quick Setup (Required First Time)

1. **Build the plugin locally:**
   - You need the Dalamud plugin development environment
   - Run: `dotnet build --configuration Release`
   - This creates `bin/Release/net9.0/EasyRaffle.dll`

2. **Create latest.zip:**
   - Create a folder called `release`
   - Copy these files into it:
     - `bin/Release/net9.0/EasyRaffle.dll`
     - `EasyRaffle.json`
     - `easyraffle.ico`
     - `easyraffle.png`
     - Any dependencies (ECommons.dll, etc.)
   - Zip all files from `release` folder into `latest.zip` in the root

3. **Push to GitHub:**
   ```bash
   git add latest.zip
   git commit -m "Add latest.zip for installation"
   git push
   ```

4. **Install in Dalamud:**
   - Go to Dalamud Settings → Experimental
   - Add repo URL: `https://raw.githubusercontent.com/GamerLatte/Easy-Raffle/main/repo.json`
   - Search for "Easy Raffle" and install

## After Initial Setup

Once `latest.zip` exists, the plugin will work directly from GitHub. The GitHub Actions workflow will attempt to auto-build on future pushes, but you may need to build locally if Dalamud SDK references aren't available in the CI environment.

