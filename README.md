# 🎟️ Easy Raffle Plugin

<p align="center">
  <img src="easyraffle.png" alt="Easy Raffle" width="300"/>
</p>

✨ *A clean, lightweight Dalamud plugin for running elegant raffles in FFXIV. Designed for live events, with all the essentials and none of the bloat.*

**No authorization required - open source and free for everyone to use!**

---

## 🖥️ Installation & Setup

### Step 1: Enable Experimental Plugins  
In Dalamud, go to `Settings → Experimental` and add this repo URL:

```plaintext
https://raw.githubusercontent.com/GamerLatte/Easy_Raffle/main/repo.json
```

### Step 2: Install  
Search for **Easy Raffle** in the **All Plugins** section and install it.

---

## 📜 Commands

```plaintext
/raffler        → Opens the main raffle UI
/rafflerlite    → Opens the lite UI!
```

---

## ✨ Core Features

✅ **Instant Access UI**  
Open the interface from anywhere with `/raffler`.
Open the Lite UI `/rafflerlite`

✅ **Repeat Buyer Detection**  
Warns you if the same person is trying to enter tickets twice in a row.

✅ **Target Name Autofill**  
🎯 Click `@ Target` to autofill the selected player.  
💬 Click `Last /t` to autofill from your last tell.

✅ **Ticket Editing + Deletion**  
Modify or delete existing entries right inside the ticket list.

✅ **Webhook Support**  
Send full ticket data directly to Discord with a single click.

✅ **Crash-Resistant Auto Save**  
Entries are written to `raffle_entries.json` in real time - your session is always safe.

✅ **Sequential Ticketing**  
Every ticket is uniquely numbered (e.g., `1 John`, `2 John`, `3 Jane`).

✅ **Import from CSV**  
Load saved ticket data from your `Downloads` folder.

✅ **Export to CSV**  
Copy full ticket data in flat or grouped formats.

✅ **Discord Preview Mode**  
Split large ticket lists into 4000-character chunks and copy them cleanly for Discord posts.

✅ **Raffle Macros**  
Auto-generate venue shout macros with live ticket counts, gil totals, and bonus status.

✅ **Session Metrics**  
Track total gil earned, ticket count, and rate stats live during your event.

✅ **Configurable Starting Pot**  
Set an initial gil pool and track how it grows as entries roll in.

✅ **Safe Reset Dialog**  
Confirm before wiping your current raffle - just in case.

---

## 🔧 Config Options

- 💵 Starting Pot (Millions)
- 🎫 Ticket Cost
- 🔒 Lock Bonus Settings After Start
- 🌐 Discord Webhook URL
- 🧱 Movable Config Window

---

## 🛠️ Building from Source

### Prerequisites
- .NET SDK 9.0 or later
- Dalamud plugin development environment

### Build Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/GamerLatte/Easy_Raffle.git
   cd Easy_Raffle
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the plugin:
   ```bash
   dotnet build --configuration Release
   ```

4. The compiled DLL will be in `bin/Release/net9.0/EasyRaffle.dll` (note: AssemblyName is EasyRaffle)

---

## 📝 Changes from Original

This is a fork of Nilah's Raffle Plugin with the following modifications:

- ✅ **Removed authorized user list requirement** - All users can now use the plugin without restrictions
- ✅ **No external API calls** - No dependency on GitHub for user authorization
- ✅ **Open source** - Full source code available for modification and distribution

---

## 📄 License

This project is open source. Please check the LICENSE file for details.

---

## 🙏 Credits

Original plugin by Nilah Valoryn.  
Modified and open-sourced version - Easy Raffle.

---

## 🛡️ Disclaimer

This plugin is not affiliated with Square Enix or Final Fantasy XIV. Use at your own risk.

