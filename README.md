# Space Engineers Discord API Mod & Relay

This project consists of two components:
1. **Space Engineers Mod**: Monitors game events (chat, connections, deaths) and writes temporary JSON event payloads into the world's local storage directory (`Storage`).
2. **External Relay (DiscordRelay)**: A lightweight .NET console application that watches the mod's storage directory, reads queued events, and dispatches them to Discord via Webhooks while respecting Discord's rate limits.

---

## 🛠️ Space Engineers Mod

### Installation / Local Development Deployment
Space Engineers automatically compiles mod scripts at runtime. There is no pre-compilation step needed for the mod scripts.

To deploy the mod locally for testing:
1. Create a folder named `DiscordAPI` in the game's local mod directory:
   `%AppData%\Roaming\SpaceEngineers\Mods\DiscordAPI\`
2. Copy the `Data` directory from this repository into that folder, resulting in this folder structure:
   ```
   %AppData%\Roaming\SpaceEngineers\Mods\DiscordAPI\Data\Scripts\DiscordAPI\...
   ```
3. Enable the **DiscordAPI** mod in your Space Engineers world settings.

---

## 🚀 External Relay (DiscordRelay)

Since the Space Engineers game sandbox blocks direct HTTP requests for security reasons (and Steam Workshop removes `.exe` files from published mods), this companion console application bridges the game storage files with Discord's APIs.

### 📥 Production Download (GitHub Releases)
For dedicated server administrators and players running production worlds:

```powershell
New-Item -ItemType Directory -Force "DiscordAPI" | Out-Null; Set-Location "DiscordAPI"; @("DiscordRelay.exe", "relay_config.example.json", "README.md", "README.txt") | ForEach-Object { Invoke-WebRequest "https://github.com/Bastion-Austral/discord-api-se1/releases/download/v1.0.0/$_" -OutFile ".\$_" }
```
Or download manually from [GitHub Releases](https://github.com/Bastion-Austral/discord-api-se1/releases/tag/v1.0.0).

---

### Environment Build Guidelines (Development vs. Production)

If you are developing or compiling from source:
*   [.NET Core SDK](https://dotnet.microsoft.com/download) (Version 6.0 or higher).

#### 🧪 For Development & Testing
*   **Compile in Debug mode:**
    ```bash
    cd ExternalRelay
    dotnet build -c Debug
    ```
*   **Run directly in Development:**
    ```bash
    cd ExternalRelay
    dotnet run
    ```

#### 📦 For Production & Standalone Executable
*   **Compile single-file Release executable:**
    ```powershell
    cd ExternalRelay
    powershell -ExecutionPolicy Bypass -File .\publish.ps1
    ```

---

## ⚙️ Configuration & Setup

### 1. Configure `relay_config.json`
Edit the `relay_config.json` file located next to the Relay executable:

```json
{
  "QueueDirectory": "C:\\Users\\<USER>\\AppData\\Roaming\\SpaceEngineers\\Saves\\<YOUR_STEAM_ID>\\<WORLD_NAME>\\Storage\\Discord API_DiscordAPI",
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

*   **`QueueDirectory`**: The absolute path to the mod's specific save folder. **It is critical that this path points exactly to the `Discord API_DiscordAPI` directory inside the `Storage` folder of your active world save.**
*   **`DefaultDiscordUrl`**: The default fallback Webhook URL.
*   **`Channels`**: Allows segmenting chat message events, connections/disconnections, and death notices to separate Discord channels.

### 2. Execution
1. Start your **Space Engineers** game/server with the mod activated.
2. Launch the **DiscordRelay**:
   * For production: Execute the pre-compiled `DiscordRelay.exe` directly (self-contained, no .NET install required).
   * For development: Run `dotnet run` inside the `ExternalRelay` directory.
3. Done! Events will automatically queue up and stream to your configured Discord webhooks.

---

## 🛡️ Security & Integrity Verification

To protect users against third-party re-uploads or malicious modifications, verify the cryptographic signature (SHA-256 hash) against the official hash displayed on [GitHub Releases](https://github.com/Bastion-Austral/discord-api-se1/releases/tag/v1.0.0).

### Verification via PowerShell:
1. Open Windows PowerShell in your relay folder.
2. Run:
   ```powershell
   Get-FileHash -Path .\DiscordRelay.exe -Algorithm SHA256
   ```
3. Compare with the official hash from GitHub:
   * **v1.0.0**: `274F04AD53620F07152BE2500836D544090E67D3BD128B2F55688F3ED43BE62D`

*If the hash does not match the official release on GitHub, DO NOT execute the file and delete it immediately.*
