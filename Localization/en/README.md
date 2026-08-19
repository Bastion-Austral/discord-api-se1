[h1]Space Engineers Discord API[/h1]

[b]Space Engineers Discord API[/b] is a high-performance companion utility that bridges your game server's internal events directly to your Discord channels via Webhooks. It acts as an API provider, logging chat messages, connections, and death events with zero performance impact.

[hr]

[h2]🪐 How It Works[/h2]

Space Engineers limits direct internet requests within scripts for security reasons. This mod bypasses this limitation safely:
[list]
[*] [b]1. In-game logger:[/b] The mod runs silently on the server, writing events (chat, connections, deaths) into a temporary queue file inside the world's [i]Storage[/i] folder.
[*] [b]2. External Relay:[/b] A lightweight, optimized console app ([i]DiscordRelay.exe[/i]) watches the queue folder and transmits the messages directly to Discord while respecting rate limits.
[/list]

[hr]

[h2]📡 Configurable Event Channels[/h2]

You can redirect different events to separate Discord channels in your [i]relay_config.json[/i]:
[list]
[*] [b]Chat Channel:[/b] Logs all in-game public chat messages.
[*] [b]Connections Channel:[/b] Notifies when players join or leave the server.
[*] [b]Deaths Channel:[/b] Logs player deaths, including weapon types and attackers.
[/list]

[hr]

[h2]⚠️ DISCLAIMER OF LIABILITY (Exclusión de Responsabilidad)[/h2]
[b]IMPORTANT SECURITY NOTICE:[/b]
[list]
[*] This mod package contains an external companion executable ([i]DiscordRelay.exe[/i]).
[*] [b]Official Release Only:[/b] The creators of this mod are ONLY responsible for the files distributed through the official Steam Workshop page and official GitHub repository.
[*] [b]Malicious Re-uploads:[/b] We are not liable or responsible for any damage, server data loss, or security compromises caused by running modified, cloned, or unofficial versions of the executable downloaded from third-party re-uploads.
[*] [b]Verify Your Files:[/b] Always verify the SHA-256 hash of the executable before running it. If you do not trust pre-compiled files, you can compile it yourself using the provided [i]publish.ps1[/i] script and the open-source code included in the folder.
[/list]

[hr]

[h2]🛡️ Cryptographic Verification (v1.0.0)[/h2]
Ensure your executable is official by running this in PowerShell:
[code]
Get-FileHash -Path .\DiscordRelay.exe -Algorithm SHA256
[/code]
Compare it to the official release signature:
[list]
[*] [b]v1.0.0 Hash:[/b] [i]274F04AD53620F07152BE2500836D544090E67D3BD128B2F55688F3ED43BE62D[/i]
[/list]
