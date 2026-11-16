# Crypte van de Domtoren

Dungeon crawler in C# en .NET voor Framework leerjaar 2, periode 2.

## Week 1: projecten starten
- `RPG_API`: ASP.NET Core Web API met het endpoint `GET /api/status`.
- `RPG_Console`: console-applicatie met een menu (*Nieuw spel*, *Afsluiten*).

De console en de API zijn twee losse programma's. Ze praten nog niet met elkaar.

## Starten
Twee terminals, allebei vanuit de hoofdmap:

```powershell
dotnet run --project .\RPG_API
```
```powershell
dotnet run --project .\RPG_Console
```

Controleer de API met `http://localhost:5265/api/status`.
