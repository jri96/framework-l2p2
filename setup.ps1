# Eenmalige setup. Uitvoeren in PowerShell vanuit de hoofdmap van het project:
#   .\setup.ps1
# Zorg dat MySQL in WAMP draait (groen icoon).
$ErrorActionPreference = "Stop"

Write-Host "1/4 dotnet-ef tool installeren (lokaal, versie 9.0.0)..." -ForegroundColor Cyan
dotnet tool restore

Write-Host "2/4 NuGet-packages herstellen..." -ForegroundColor Cyan
dotnet restore .\RPG_API\RPG_API.csproj
dotnet restore .\RPG_Console\RPG_Console.csproj

if (-not (Test-Path .\RPG_API\Migrations)) {
    Write-Host "3/4 Eerste migration maken (InitialGame)..." -ForegroundColor Cyan
    dotnet ef migrations add InitialGame --project .\RPG_API --startup-project .\RPG_API
} else {
    Write-Host "3/4 Migrations bestaan al, overslaan." -ForegroundColor Cyan
}

Write-Host "4/4 Database DungeonCrawler aanmaken/bijwerken..." -ForegroundColor Cyan
dotnet ef database update --project .\RPG_API --startup-project .\RPG_API

Write-Host ""
Write-Host "Klaar! Start nu in twee terminals:" -ForegroundColor Green
Write-Host "  dotnet run --project .\RPG_API"
Write-Host "  dotnet run --project .\RPG_Console"
