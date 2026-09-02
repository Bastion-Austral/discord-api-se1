# DiscordRelay - External Webhook Bridge

This is the companion console application for the **Space Engineers Discord API Mod**. Since the game's script sandbox blocks direct HTTP requests for security reasons, this lightweight program bridges the game's local storage event files with Discord's webhook API.

> [!NOTE]
> **Steam Workshop Notice:** Steam Workshop automatically strips `.exe` binaries upon mod upload. For production deployments, download `DiscordRelay.exe` directly from our official [GitHub Releases](https://github.com/Bastion-Austral/discord-api-se1/releases/tag/v1.0.0).

---

## 📥 Quick Download (PowerShell)

To automatically create a `DiscordAPI` folder and download all required companion files:

```powershell
New-Item -ItemType Directory -Force "DiscordAPI" | Out-Null; Set-Location "DiscordAPI"; @("DiscordRelay.exe", "relay_config.example.json", "README.md", "README.txt") | ForEach-Object { Invoke-WebRequest "https://github.com/Bastion-Austral/discord-api-se1/releases/download/v1.0.0/$_" -OutFile ".\$_" }
```

---

## ⚙️ Configuration

Before running the executable, you must configure `relay_config.json`. You can rename `relay_config.example.json` to get started.

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

- **`QueueDirectory`**: The absolute path to the mod's specific save folder. **It must point exactly to the `Discord API_DiscordAPI` directory inside the `Storage` folder of your active world save.**
- **`DefaultDiscordUrl`**: The default fallback Webhook URL.
- **`Channels`**: Redirects specific events (chat, connections, deaths) to separate Discord channels.

---

## 🛡️ Security & Integrity Verification

To guarantee that this binary has not been modified by a third party, you can verify its cryptographic signature (SHA-256 hash) against the official hash displayed on [GitHub Releases](https://github.com/Bastion-Austral/discord-api-se1/releases/tag/v1.0.0).

### Step-by-Step Verification Guide for Beginners:
1. **Open the Terminal (PowerShell):**
   * Press the **Windows Key** on your keyboard.
   * Type **PowerShell** and click on **Windows PowerShell** to open it.
2. **Navigate to your mod folder:**
   * Open your File Explorer and find the folder where `DiscordRelay.exe` is located.
   * In the blue PowerShell window, type `cd ` (type the letters **c** and **d**, followed by a **space**).
   * **Drag and drop** the folder containing `DiscordRelay.exe` from your File Explorer directly into the PowerShell window.
   * Press the **Enter** key.
3. **Run the check command:**
   * Run the command below (see [Microsoft Get-FileHash documentation](https://learn.microsoft.com/powershell/module/microsoft.powershell.utility/get-filehash)):
     ```powershell
     Get-FileHash -Path .\DiscordRelay.exe -Algorithm SHA256
     ```
4. **Compare the result with GitHub:**
   * Compare the output hash on your screen with the official hash listed under the release assets on GitHub:

* **Official v1.0.0 Hash**: `274F04AD53620F07152BE2500836D544090E67D3BD128B2F55688F3ED43BE62D`

*If the hash shown in PowerShell does not match the official hash on GitHub, DO NOT run the file and delete it immediately.*

---

## 🚀 Execution & Production Deployment

To run the relay:
1. Ensure the mod is active in your Space Engineers world.
2. Run `DiscordRelay.exe` in the background (as a service or console window).
3. The application will monitor files and queue messages automatically.

---

## 🏗️ Compilation & Development

You can rebuild the executable using the provided source code:

### 🧪 1. For Development & Debugging
Run directly using the .NET SDK:
```bash
dotnet run
```
Or build the debug package:
```bash
dotnet build -c Debug
```

### 📦 2. For Production (Bake Single-File Executable)
We provide a PowerShell script to compile a highly optimized, standalone, self-contained executable for Windows (`win-x64`):
1. Open PowerShell in this directory.
2. Run:
   ```powershell
   powershell -ExecutionPolicy Bypass -File .\publish.ps1
   ```
This compiles the code into `DiscordRelay.exe` containing the full .NET runtime.
