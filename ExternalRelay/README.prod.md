# DiscordRelay - Production User Guide

Companion bridge executable for the **Space Engineers Discord API Mod**.

This standalone tool monitors event queues created by the in-game mod inside your world's save storage and delivers chat, connection, and death notifications directly to your Discord server via Webhooks.

> [!NOTE]
> **Why download from GitHub?**  
> Steam Workshop automatically removes `.exe` files from published mods for security. Therefore, the companion `DiscordRelay.exe` and its configuration files are distributed through our official [GitHub Releases](https://github.com/Bastion-Austral/discord-api-se1/releases).

---

## 📥 Download & Installation

### Option 1: Quick PowerShell Download (Recommended)
Open PowerShell in the directory where you want to keep the relay and run:

```powershell
New-Item -ItemType Directory -Force "DiscordAPI" | Out-Null; Set-Location "DiscordAPI"; @("DiscordRelay.exe", "relay_config.example.json", "README.md", "README.txt") | ForEach-Object { Invoke-WebRequest "https://github.com/Bastion-Austral/discord-api-se1/releases/download/v1.0.0/$_" -OutFile ".\$_" }
```

### Option 2: Manual Download via Browser
1. Visit the official GitHub Releases page:  
   👉 **[GitHub Releases - Space Engineers Discord API](https://github.com/Bastion-Austral/discord-api-se1/releases/latest)**
2. Under **Assets**, download:
   * `DiscordRelay.exe`
   * `relay_config.example.json`
   * `README.md` / `README.txt`
3. Place all downloaded files in a dedicated folder (e.g., `DiscordAPI/`).

---

## 🛡️ Pre-Flight Verification & Integrity Check

Before running the executable, verify the authenticity and integrity of `DiscordRelay.exe` against the official hash published on GitHub.

### 1. Checking Binary Hash (PowerShell):
1. Open **PowerShell** in your `DiscordAPI` folder.
2. Run the hash check command:
   ```powershell
   Get-FileHash -Path .\DiscordRelay.exe -Algorithm SHA256
   ```

### 2. Compare with GitHub Official Hash:
Compare the output string with the official SHA-256 hash displayed in the [GitHub Release assets list](https://github.com/Bastion-Austral/discord-api-se1/releases/tag/v1.0.0):

* **Official Hash (v1.0.0)**:
  `274F04AD53620F07152BE2500836D544090E67D3BD128B2F55688F3ED43BE62D`

> [!WARNING]
> If the hash output in PowerShell does not match the official hash from GitHub, **do not run the executable** and delete it immediately.

---

## ⚙️ Configuration Guide

1. Rename `relay_config.example.json` to **`relay_config.json`** (or copy it).
2. Open `relay_config.json` with any text editor (Notepad, VS Code, etc.).

### Configuration Schema (`relay_config.json`)
```json
{
  "QueueDirectory": "C:\\Users\\<USER>\\AppData\\Roaming\\SpaceEngineers\\Saves\\<STEAM_ID>\\<WORLD_NAME>\\Storage\\Discord API_DiscordAPI",
  "DefaultDiscordUrl": "https://discord.com/api/webhooks/...",
  "Channels": {
    "Chat": {
      "DiscordUrl": "https://discord.com/api/webhooks/..."
    },
    "Connections": {
      "DiscordUrl": "https://discord.com/api/webhooks/..."
    },
    "Deaths": {
      "DiscordUrl": "https://discord.com/api/webhooks/..."
    }
  }
}
```

### Configuration Fields:
* **`QueueDirectory`** (*Required*): Absolute path to the mod's storage directory inside the active world save.
  * **Dedicated Server**: `C:\ProgramData\SpaceEngineersDedicated\<InstanceName>\Saves\<WorldName>\Storage\Discord API_DiscordAPI`
  * **Local Host / Listen Server**: `C:\Users\<User>\AppData\Roaming\SpaceEngineers\Saves\<SteamID64>\<WorldName>\Storage\Discord API_DiscordAPI`
* **`DefaultDiscordUrl`**: Fallback Discord Webhook URL for events not routed to specific channels.
* **`Channels`** (*Optional*): Route specific event categories (`Chat`, `Connections`, `Deaths`) to distinct Discord channels.

> [!IMPORTANT]
> In JSON, all Windows directory backslashes **must be doubled** (e.g. `C:\\Users\\...` instead of `C:\Users\...`).

---

## ✅ Pre-Run Checklist

Verify every item in this checklist before starting the relay:

- [ ] **Mod Active in World**: The Discord API Mod is subscribed on Steam Workshop and enabled in your Space Engineers world.
- [ ] **Storage Folder Generated**: The world has been started at least once with the mod enabled so that the `Storage\Discord API_DiscordAPI` folder exists.
- [ ] **Config File Located**: `relay_config.json` is located in the exact same directory as `DiscordRelay.exe`.
- [ ] **Valid JSON Syntax**: Quotes are closed, backslashes are escaped (`\\`), and no trailing commas exist.
- [ ] **Valid Discord Webhooks**: Webhook URLs are created in Discord (`Channel Settings -> Integrations -> Webhooks`) and start with `https://discord.com/api/webhooks/`.
- [ ] **Queue Path Accessible**: The user running `DiscordRelay.exe` has Read/Write permissions to `QueueDirectory`.

---

## 🚀 Running the Relay

1. Launch your Space Engineers Dedicated Server or Game session.
2. Double-click **`DiscordRelay.exe`** (or run it from PowerShell/CMD).
3. The console will display startup information and monitor the event queue in real-time.
4. Keep the console window running while the game server is active.

---

## ❓ Troubleshooting

| Issue / Error Message | Cause & Solution |
| :--- | :--- |
| **"Missing required settings: QueueDirectory is empty"** | Ensure `relay_config.json` exists in the executable's folder and has the `QueueDirectory` field filled out. |
| **"Directory not found" or no events processed** | Verify that the path in `QueueDirectory` matches your active world folder and that the mod is enabled in the game save. |
| **JSON Parse Error on startup** | Check for unescaped single backslashes in Windows paths (replace `\` with `\\`) or syntax errors like trailing commas. |
| **HTTP 404 / 401 Webhook Error** | The Discord webhook URL is incorrect or was deleted in Discord. Create a new webhook in Discord channel settings and update `relay_config.json`. |
| **Console closes immediately** | Run `DiscordRelay.exe` from PowerShell/CMD to inspect the error output. |
