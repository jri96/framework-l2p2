@echo off
cd /d "%~dp0"
title Crypte van de Domtoren - setup

where dotnet >nul 2>nul
if errorlevel 1 (
  echo .NET SDK niet gevonden. Installeer eerst de .NET 10 SDK:
  echo   winget install Microsoft.DotNet.SDK.10
  pause
  exit /b 1
)

echo [1/4] dotnet-ef installeren...
dotnet tool restore || goto fout

echo [2/4] NuGet-packages herstellen...
dotnet restore RPG_API\RPG_API.csproj || goto fout
dotnet restore RPG_Console\RPG_Console.csproj || goto fout

if not exist RPG_API\Migrations (
  echo [3/4] Eerste migration maken - InitialGame...
  dotnet ef migrations add InitialGame --project RPG_API --startup-project RPG_API || goto fout
) else (
  echo [3/4] Migrations bestaan al.
)

echo [4/4] Database bijwerken - MySQL in WAMP moet aan staan...
dotnet ef database update --project RPG_API --startup-project RPG_API || goto fout

echo API starten in een nieuw venster...
start "RPG_API" cmd /k dotnet run --project RPG_API
echo Even wachten tot de API draait...
timeout /t 10 >nul
start "RPG_Console" cmd /k dotnet run --project RPG_Console
exit /b 0

:fout
echo.
echo Er ging iets mis. Lees de melding hierboven. Staat WAMP / MySQL aan?
pause
exit /b 1
