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

Since the Space Engineers game sandbox blocks direct HTTP requests for security reasons, this external console application bridges the game storage files with Discord's APIs.

### Requirements
*   [.NET Core SDK](https://dotnet.microsoft.com/download) (Version 6.0 or higher recommended).

### Environment Build Guidelines (Development vs. Production)

Depending on your current goal, you should build the relay using one of the following configurations:

#### 🧪 For Development & Testing (Recommended for now)
Use the **Debug** configuration. This builds the relay with full debug symbols, disables compiler optimizations, and preserves clear stack traces in case something crashes. It also runs faster if you use the hot-reload runner.
*   **Compile in Debug mode:**
    ```bash
    cd ExternalRelay
    dotnet build -c Debug
    ```
    The compiled binary will be located in: `ExternalRelay/bin/Debug/net6.0/DiscordRelay.exe`.
*   **Run directly in Development:**
    ```bash
    cd ExternalRelay
    dotnet run
    ```

#### 📦 For Production & Live Servers
Use the **Release** configuration. This enables full compiler optimizations, resulting in a significantly smaller binary footprint and maximum execution performance with minimal CPU/RAM overhead while running indefinitely.
*   **Compile in Release mode:**
    ```bash
    cd ExternalRelay
    dotnet build -c Release
    ```
    The compiled binary will be located in: `ExternalRelay/bin/Release/net6.0/DiscordRelay.exe`.

---

## ⚙️ Configuration & Setup

### 1. Configure `relay_config.json`
Edit the `relay_config.json` file located next to the Relay executable:

```json
{
  "QueueDirectory": "C:\\Users\\dmira\\AppData\\Roaming\\SpaceEngineers\\Saves\\<YOUR_STEAM_ID>\\<WORLD_NAME>\\Storage\\Discord API_DiscordAPI",
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

To protect users against third-party re-uploads or malicious modifications, we provide cryptographic verification of the official release binary.

### 1. Verification via Hash (SHA-256) (For Beginners)
To guarantee that this binary has not been modified by a third party, you can check its cryptographic signature (SHA-256 hash) before running it:

1. **Open Windows PowerShell:**
   * Press the **Windows Key** on your keyboard.
   * Type **PowerShell** and click on **Windows PowerShell** to open it.
2. **Go to the folder where the file is located:**
   * In File Explorer, find the folder containing `DiscordRelay.exe`.
   * In the blue PowerShell window, type `cd ` (type the letters **c** and **d**, followed by a **space**).
   * **Drag and drop** the folder containing `DiscordRelay.exe` from your File Explorer directly into the PowerShell window. The computer will type the full path for you!
   * Press the **Enter** key.
3. **Run the check command:**
   * Copy the command below, paste it into the PowerShell window, and press **Enter** (you can read more about what this command does in the [official Microsoft Get-FileHash documentation](https://learn.microsoft.com/powershell/module/microsoft.powershell.utility/get-filehash)):
     ```powershell
     Get-FileHash -Path .\DiscordRelay.exe -Algorithm SHA256
     ```
4. **Compare the result:**
   * Compare the long string of numbers and letters shown on your screen with the official hash for your version:

* **v1.0.0**: `274F04AD53620F07152BE2500836D544090E67D3BD128B2F55688F3ED43BE62D`

*If the hash does not match, DO NOT execute the file and delete it immediately.*

### 2. Self-Compilation (Zero Trust)
If you prefer not to run pre-compiled binaries, you can compile the executable yourself using the included source code:
1. Open PowerShell in the `ExternalRelay` directory.
2. Run the script:
   ```powershell
   powershell -ExecutionPolicy Bypass -File .\publish.ps1
   ```
This will compile a secure, optimized, single-file executable directly from the source code on your machine.

