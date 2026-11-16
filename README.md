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

## Week 3: API-structuur
- De endpoints staan nu in controllers: `StatusController` en `InfoController`.
- De gegevens komen uit services achter een interface: `IStatusService` en `IGameInfoService`.
- De services worden geregistreerd in `Program.cs` (`AddScoped`) en via de constructor in de controller geïnjecteerd.
- Het requestpad (`/api/status`, `/api/info`) en de DTO-velden zijn gelijk gebleven, dus de console werkt zonder aanpassing.
- De taakverdeling voor de rest van het spel staat in [docs/TAKEN.md](docs/TAKEN.md).

## Week 4: database en spel starten
- EF Core met MySQL (Pomelo). Entities `Player` en `Game`, `GameDbContext`.
- `POST /api/game` slaat een nieuwe speler en game op en geeft **201 Created** met een `GameResponse` terug.
- `GET /api/game` en `GET /api/game/{id}` halen opgeslagen spellen op, met `Include(game => game.Player)`.
- De console kan een spel starten en na een herstart hervatten.
- Relatie en gekozen velden: [docs/ERD.md](docs/ERD.md).

## Week 5: bewegen, rustplaats en uitgang
- `POST /api/game/{id}/move`: de `MovementService` volgt steeds dezelfde stappen: game ophalen → controleren → volgend level en ruimte bepalen → opslaan → response.
- `RoomType`: `Start`, `Normal`, `Treasure`, `Rest`, `Exit`.
- Tien levels. Rustplaats op level 5 en 9 (volledig genezen). Level 10 is de uitgang: het spel is gewonnen en afgelopen.
- Andere levels: 30% kans op een schatkamer (genezingsdrankje), anders een lege ruimte.
- Een afgelopen spel kan nog bekeken worden, maar krijgt geen nieuwe acties.

### Eigen spelregel: genezingsdrankje (`POST /api/game/{id}/potion`)
Voorwaarden, vooraf opgeschreven:
1. Het spel loopt nog.
2. De speler heeft minstens één drankje (start met 2, maximaal 5).
3. De speler heeft geen volle HP; anders wordt de actie geweigerd.
4. Een drankje geneest 30 HP, maar nooit boven het maximum.

Getest: onbekende game en een actie na het einde van het spel worden geweigerd. Ze geven nu nog een 500-fout; in week 7 worden dat nette statuscodes (404 en 409).

## Starten
WAMP aan (MySQL), daarna dubbelklik op `start.bat`. Dat doet eenmalig `dotnet tool restore`, `dotnet restore`, maakt de migration `InitialGame` en de database, en start daarna API en console.

Met de hand:

```powershell
dotnet ef migrations add InitialGame --project RPG_API --startup-project RPG_API
dotnet ef database update --project RPG_API --startup-project RPG_API
```

Daarna twee terminals, allebei vanuit de hoofdmap:

```powershell
dotnet run --project .\RPG_API
```
```powershell
dotnet run --project .\RPG_Console
```

Controleer de API met `http://localhost:5265/api/status`.
