# Building the Plugin Manually

If you need to build the plugin manually before the GitHub Action runs, follow these steps:

## Prerequisites
- .NET SDK 9.0 or later
- Dalamud plugin development environment set up
- ECommons.dll (will be included automatically if referenced)

## Build Steps

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Build the plugin:**
   ```bash
   dotnet build --configuration Release
   ```

3. **Create the release package:**
   
   Create a folder called `release` and copy these files into it:
   - `bin/Release/net9.0/EasyRaffle.dll`
   - `bin/Release/net9.0/EasyRaffle.pdb` (optional)
   - `EasyRaffle.json`
   - `easyraffle.ico`
   - `easyraffle.png`
   - Any dependencies (ECommons.dll, etc.)

4. **Create latest.zip:**
   
   Zip all files from the `release` folder into `latest.zip` in the root directory.

5. **Commit and push:**
   ```bash
   git add latest.zip
   git commit -m "Add latest.zip"
   git push
   ```

## Automatic Building

The GitHub Actions workflow will automatically build and create `latest.zip` whenever you push to the main branch. The file will be committed back to the repository automatically.

