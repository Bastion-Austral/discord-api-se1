DISCORD RELAY - EXTERNAL WEBHOOK BRIDGE
=======================================

This is the companion console application for the Space Engineers Discord API Mod. 
Since the game's script sandbox blocks direct HTTP requests for security reasons, 
this lightweight program bridges the game's local storage event files with 
Discord's webhook API.

NOTE: Steam Workshop automatically strips .exe binaries upon mod upload. For 
production deployments, download DiscordRelay.exe directly from our official 
GitHub Releases:
https://github.com/Bastion-Austral/discord-api-se1/releases/tag/v1.0.0

-------------------------------------------------------------------------------
1. QUICK DOWNLOAD (POWERSHELL)
-------------------------------------------------------------------------------
To automatically create a 'DiscordAPI' folder and download all companion files:

New-Item -ItemType Directory -Force "DiscordAPI" | Out-Null; Set-Location "DiscordAPI"; @("DiscordRelay.exe", "relay_config.example.json", "README.md", "README.txt") | ForEach-Object { Invoke-WebRequest "https://github.com/Bastion-Austral/discord-api-se1/releases/download/v1.0.0/$_" -OutFile ".\$_" }

-------------------------------------------------------------------------------
2. CONFIGURATION
-------------------------------------------------------------------------------
Before running the executable, you must configure 'relay_config.json'. 
You can rename 'relay_config.example.json' to get started.

Configuration Schema ('relay_config.json'):
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

- QueueDirectory: The absolute path to the mod's specific save folder. 
  It must point exactly to the "Discord API_DiscordAPI" directory inside the 
  "Storage" folder of your active world save.
- DefaultDiscordUrl: The default fallback Webhook URL.
- Channels: Redirects specific events (chat, connections, deaths) to separate 
  Discord channels.

-------------------------------------------------------------------------------
3. SECURITY & INTEGRITY VERIFICATION
-------------------------------------------------------------------------------
To guarantee that this binary has not been modified by a third party, you can 
verify its cryptographic signature (SHA-256 hash) against the official hash 
published on GitHub Releases (https://github.com/Bastion-Austral/discord-api-se1/releases/tag/v1.0.0).

Step-by-Step Verification Guide for Beginners:
1. Open the Terminal (PowerShell):
   - Press the Windows Key on your keyboard.
   - Type "PowerShell" and click on "Windows PowerShell" to open it.
2. Navigate to your mod folder:
   - Open your File Explorer and find the folder where DiscordRelay.exe is located.
   - In the blue PowerShell window, type: cd  (type "cd" followed by a space).
   - Drag and drop the folder containing DiscordRelay.exe from File Explorer.
   - Press the Enter key.
3. Run the check command:
   Get-FileHash -Path .\DiscordRelay.exe -Algorithm SHA256

4. Compare the result with GitHub:
   Compare the long string of numbers and letters (the Hash) shown on your 
   screen with the official hash from GitHub:

   Official v1.0.0 Hash:
   274F04AD53620F07152BE2500836D544090E67D3BD128B2F55688F3ED43BE62D

* If the hash shown in PowerShell does not match the official hash on GitHub, 
  DO NOT run the file and delete it immediately.

-------------------------------------------------------------------------------
4. EXECUTION & PRODUCTION DEPLOYMENT
-------------------------------------------------------------------------------
To run the relay:
1. Ensure the mod is active in your Space Engineers world.
2. Run 'DiscordRelay.exe' in the background (as a service or console window).
3. The application will monitor files and queue messages automatically.

-------------------------------------------------------------------------------
5. SELF-COMPILATION (ZERO TRUST)
-------------------------------------------------------------------------------
If you prefer not to run pre-compiled binaries, you can compile the executable 
yourself using the included source code:
1. Open PowerShell in the 'ExternalRelay' directory.
2. Run the script:
   powershell -ExecutionPolicy Bypass -File .\publish.ps1
   
This will compile a secure, optimized, single-file executable directly from the 
source code on your machine.
