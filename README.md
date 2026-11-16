# Crypte van de Domtoren

Dungeon crawler in C# en .NET voor Framework leerjaar 2, periode 2.

## Week 1: projecten starten
- `RPG_API`: ASP.NET Core Web API met het endpoint `GET /api/status`.
- `RPG_Console`: console-applicatie met een menu (*Nieuw spel*, *Afsluiten*).

De console en de API zijn twee losse programma's.

## Week 2: HTTP-communicatie
- Endpoint `GET /api/info` met spelinformatie (titel, omschrijving, aantal levels).
- De console gebruikt `HttpClient` en `GetFromJsonAsync` om status en spelinformatie op te halen.
- Het hoofdmenu laat zien of de API online is. Staat de API uit, dan toont de console een melding in plaats van te crashen.

Voorbeeld van één request:

| Onderdeel | Waarde |
| --- | --- |
| Client | `RPG_Console` |
| Server | `RPG_API` |
| Methode | `GET` |
| URL | `http://localhost:5265/api/info` |
| Statuscode | `200 OK` |
| JSON | `{ "title": "Crypte van de Domtoren", "description": "...", "maxLevel": 10 }` |

## Starten
Twee terminals, allebei vanuit de hoofdmap:

```powershell
dotnet run --project .\RPG_API
```
```powershell
dotnet run --project .\RPG_Console
```

Controleer de API met `http://localhost:5265/api/status`.
