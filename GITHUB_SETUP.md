# GitHub Setup Guide

## Initial Setup

1. **Create a new repository on GitHub**
   - Go to https://github.com/new
   - Name it `Easy-Raffle`
   - Make it public (or private if you prefer)
   - Do NOT initialize with README, .gitignore, or license (we already have these)

2. **Update repository URLs**
   - All GitHub username references have been set to `GamerLatte`

3. **Initialize Git and push**
   ```bash
   cd C:\Users\bigbr\Documents\Cursor\Easy_Raffle
   git init
   git add .
   git commit -m "Initial commit - Easy Raffle plugin without authorization"
   git branch -M main
   git remote add origin https://github.com/GamerLatte/Easy-Raffle.git
   git push -u origin main
   ```

## Creating Releases

To make the plugin available for download through Dalamud:

1. **Build the plugin**
   ```bash
   dotnet build --configuration Release
   ```

2. **Create a release package**
   - Copy the following files to a folder:
     - `bin/Release/net9.0/EasyRaffle.dll`
     - `bin/Release/net9.0/EasyRaffle.pdb` (optional)
     - `EasyRaffle.json` (plugin manifest)
     - `easyraffle.ico`
     - `easyraffle.png`
     - Any dependencies (ECommons.dll, etc.)
   - Zip these files into `latest.zip`

3. **Create a GitHub Release**
   - Go to your repository on GitHub
   - Click "Releases" → "Create a new release"
   - Tag version: `v2.5.0.0`
   - Release title: `Easy Raffle v2.5.0.0`
   - Upload `latest.zip` as a release asset
   - Publish the release

4. **Update repo.json**
   - The `DownloadLinkInstall` and `DownloadLinkUpdate` should point to:
     `https://github.com/GamerLatte/Easy-Raffle/releases/latest/download/latest.zip`

## Updating the Plugin

1. Make your changes
2. Update the version number in:
   - `EasyRaffle.json` (AssemblyVersion)
   - `repo.json` (AssemblyVersion and Changelog)
3. Build and create a new release following the steps above

## Notes

- The `repo.json` file is what Dalamud uses to discover and install your plugin
- Make sure `latest.zip` is always available at the release download URL
- Update the changelog in `repo.json` for each release

