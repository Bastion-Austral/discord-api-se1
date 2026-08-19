[h1]Space Engineers Discord API[/h1]

[b]Space Engineers Discord API[/b] es una utilidad complementaria de alto rendimiento que conecta los eventos internos de tu servidor de juego directamente con tus canales de Discord a través de Webhooks. Actúa como proveedor de API, registrando mensajes de chat, conexiones y eventos de muerte con impacto cero en el rendimiento.

[hr]

[h2]🪐 Cómo funciona[/h2]

Space Engineers limita las solicitudes de internet directas dentro de los scripts por razones de seguridad. Este mod supera esta limitación de manera segura:
[list]
[*] [b]1. Registro en el juego:[/b] El mod se ejecuta de forma silenciosa en el servidor, escribiendo los eventos (chat, conexiones, muertes) en un archivo de cola temporal dentro de la carpeta [i]Storage[/i] del mundo.
[*] [b]2. Relé Externo:[/b] Una aplicación de consola ligera y optimizada ([i]DiscordRelay.exe[/i]) monitorea la carpeta de cola y transmite los mensajes directamente a Discord respetando los límites de frecuencia (rate limits).
[/list]

[hr]

[h2]📡 Canales de Eventos Configurables[/h2]

Puedes redirigir diferentes eventos a canales de Discord separados en tu [i]relay_config.json[/i]:
[list]
[*] [b]Canal de Chat:[/b] Registra todos los mensajes de chat público del juego.
[*] [b]Canal de Conexiones:[/b] Notifica cuando los jugadores entran o salen del servidor.
[*] [b]Canal de Muertes:[/b] Registra las muertes de los jugadores, incluyendo tipos de armas y atacantes.
[/list]

[hr]

[h2]⚠️ EXCLUSIÓN DE RESPONSABILIDAD (Disclaimer)[/h2]
[b]AVISO DE SEGURIDAD IMPORTANTE:[/b]
[list]
[*] Este paquete de mod contiene un ejecutable externo complementario ([i]DiscordRelay.exe[/i]).
[*] [b]Solo Lanzamientos Oficiales:[/b] Los creadores de este mod SOLO nos hacemos responsables de los archivos distribuidos a través de la página oficial de Steam Workshop y el repositorio oficial de GitHub.
[*] [b]Resubidas Maliciosas:[/b] No nos hacemos responsables de ningún daño, pérdida de datos del servidor o brechas de seguridad causadas por la ejecución de versiones modificadas, clonadas o no oficiales del ejecutable descargadas de resubidas de terceros.
[*] [b]Verifica tus archivos:[/b] Comprueba siempre el hash SHA-256 del ejecutable antes de iniciarlo. Si no confías en los archivos pre-compilados, puedes compilarlo tú mismo utilizando el script [i]publish.ps1[/i] provisto y el código fuente abierto incluido en la carpeta.
[/list]

[hr]

[h2]🛡️ Verificación Criptográfica (v1.0.0)[/h2]
Asegúrate de que tu ejecutable es el oficial ejecutando esto en PowerShell:
[code]
Get-FileHash -Path .\DiscordRelay.exe -Algorithm SHA256
[/code]
Compara el resultado con la firma oficial de la versión:
[list]
[*] [b]v1.0.0 Hash:[/b] [i]274F04AD53620F07152BE2500836D544090E67D3BD128B2F55688F3ED43BE62D[/i]
[/list]
