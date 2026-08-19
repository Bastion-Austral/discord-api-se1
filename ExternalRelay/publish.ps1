# powershell -ExecutionPolicy Bypass -File publish.ps1
# Script de publicación para DiscordRelay
# Compila el código en un único archivo ejecutable auto-contenido (no requiere instalar .NET en el servidor)

$ProjectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ProjectDir

Write-Host "Iniciando publicación de DiscordRelay..." -ForegroundColor Cyan

# Directorio temporal de salida
$OutputDir = Join-Path $ProjectDir "PublishTemp"

# Comando de publicación de .NET
# -c Release: Compila en modo producción optimizado
# -r win-x64: Especifica arquitectura Windows 64-bit
# --self-contained true: Incluye el runtime de .NET dentro del exe
# -p:PublishSingleFile=true: Empaqueta todo en un único archivo .exe
# -p:PublishReadyToRun=true: Pre-compila el código para acelerar el inicio
dotnet publish DiscordRelay.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o $OutputDir

if ($LASTEXITCODE -eq 0) {
    # Mueve el ejecutable final a la raíz de la carpeta ExternalRelay
    $SourceExe = Join-Path $OutputDir "DiscordRelay.exe"
    $DestExe = Join-Path $ProjectDir "DiscordRelay.exe"
    
    if (Test-Path $SourceExe) {
        Copy-Item -Path $SourceExe -Destination $DestExe -Force
        Write-Host "`n¡Éxito! Ejecutable único copiado a: $DestExe" -ForegroundColor Green
        
        # Calcular e imprimir el hash SHA-256
        $Hash = (Get-FileHash -Path $DestExe -Algorithm SHA256).Hash
        Write-Host "SHA-256 Hash del binario oficial:" -ForegroundColor Cyan
        Write-Host "$Hash" -ForegroundColor Yellow
    } else {
        Write-Error "No se encontró el ejecutable generado en $SourceExe"
    }
} else {
    Write-Error "La publicación de dotnet falló. Revisa los errores superiores."
}

# Limpieza del directorio temporal
if (Test-Path $OutputDir) {
    Remove-Item -Path $OutputDir -Recurse -Force
}
