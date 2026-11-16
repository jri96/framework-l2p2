# Crypte van de Domtoren

Een turn-based dungeon crawler in C# en .NET voor Framework leerjaar 2, periode 2.
De speler klimt in tien levels door een crypte naar de klokkenzolder en verslaat daar de Klokkenwachter.

```text
Console (RPG_Console)  →  HTTP/JSON  →  Controller  →  Service  →  EF Core  →  MySQL
```

De console regelt invoer, menu's en weergave. De API bevat alle spelregels en slaat de toestand op.
**De console benadert de database nooit rechtstreeks.**

---

## Starten

### Benodigd
- .NET 10 SDK (`dotnet --version` begint met `10.`)
- WAMP met MySQL aan (poort 3306, gebruiker `root`, geen wachtwoord)
- VS Code met de extensie C# Dev Kit

### Eerste keer (setup)
Zet WAMP aan en **dubbelklik op `start.bat`**. Of in PowerShell vanuit de hoofdmap: `.\setup.ps1`.

Het script doet:
1. `dotnet tool restore`: installeert `dotnet-ef` 9.0.0
2. `dotnet restore`: haalt de NuGet-packages op
3. `dotnet ef migrations add InitialGame ...`: maakt de eerste migration in `RPG_API/Migrations` (alleen als die nog niet bestaat)
4. `dotnet ef database update ...`: maakt de database `DungeonCrawler` met de tabellen en de vijanden (seeding)

Mag een script niet draaien? Voer dan eerst `Set-ExecutionPolicy -Scope Process Bypass` uit, of voer de commando's uit `setup.ps1` met de hand uit.

### Spelen
Twee terminals, allebei vanuit de hoofdmap:

```powershell
dotnet run --project .\RPG_API
```
```powershell
dotnet run --project .\RPG_Console
```

De API draait op `http://localhost:5265`. Controleer dat met `http://localhost:5265/api/status` in de browser.

### Debuggen in VS Code
Kies in *Run and Debug* de configuratie **API + Console**. Dan starten beide projecten met de debugger, zodat breakpoints in de controller, de services en de console werken. Zie [docs/DEBUGGEN.md](docs/DEBUGGEN.md).

### Na een wijziging aan een entity
```powershell
dotnet ef migrations add NaamVanDeWijziging --project .\RPG_API --startup-project .\RPG_API
dotnet ef database update --project .\RPG_API --startup-project .\RPG_API
```
De API voert bij het starten ook zelf openstaande migrations uit (`ApplyMigrations` in `Program.cs`).

---

## Wat kan de speler?

| Eis uit de opdracht | Waar |
| --- | --- |
| Nieuw spel starten en opgeslagen spel hervatten | Hoofdmenu, `POST /api/game`, `GET /api/game`, `GET /api/game/{id}` |
| Door levels bewegen, verschillende ruimtes | `MovementService` |
| Gevecht met aanvallen en tegenaanval | `CombatRoundService`, `CombatService` |
| Rustplaats, winst en verlies | Rustplaats op level 5 en 9, winst na de eindbaas, verlies bij 0 HP |
| Toestand opslaan en via de API teruggeven | Tabel `Games` + `Players`, `GameResponse`-DTO |
| Begrijpelijke foutmeldingen | `ExceptionHandlingMiddleware` + eigen exceptions |
| Request-logging | `RequestLoggingMiddleware` → tabel `ApiRequestLogs`, `GET /api/logs` |

---

## Spelregels

### Levels
| Level | Locatie | Ruimte |
| --- | --- | --- |
| 1 | Ingang van de crypte | Startpunt |
| 2-4 | Vochtige gang, Botkamer, Vergeten kapel | Willekeurig |
| 5 | Kaarsenkamer | **Rustplaats**: volledig genezen |
| 6-8 | Wenteltrap, Overstroomde kelder, Uurwerkzaal | Willekeurig |
| 9 | Torenkamer | **Rustplaats**: volledig genezen |
| 10 | Klokkenzolder | **Eindbaas**: de Klokkenwachter. Daarna ga je door de **uitgang** en heb je gewonnen |

Een willekeurige ruimte is in 65% van de gevallen een gevecht, in 20% een schatkamer (een genezingsdrankje, of +15 HP als je tas vol zit) en in 15% leeg.

### Vijanden
| Vijand | HP | Schade | Snelheid | Bijzonder |
| --- | --- | --- | --- | --- |
| Skelet | 40 | 8 | 10 | |
| Kobold | 30 | 6 | 16 | Snel, valt vaak eerst aan |
| Ork | 65 | 12 | 6 | Traag maar sterk |
| Klokkenwachter | 120 | 12 | 12 | Eindbaas, vluchten kan niet |

### Gevecht
- **Lichte aanval**: schade = aanval −2 tot +2, snelheid +10.
- **Zware aanval**: schade = aanval +3 tot +7, geen snelheidsbonus.
- De vijand kiest zelf willekeurig licht of zwaar.
- Wie de hoogste snelheid heeft, slaat eerst. Bij gelijkspel slaat de speler eerst.
- Gaat de vijand neer, dan komt er geen tegenaanval. Ga jij neer, dan is het game over.

### Eigen spelregels (met voorwaarden)
1. **Genezingsdrankje** (eigen spelactie, `POST /api/game/{id}/potion`)
   - Je start met 2 drankjes en kunt er maximaal 5 dragen.
   - Mag alleen als het spel nog loopt, je minstens één drankje hebt en je niet al volle HP hebt (anders `409 Conflict`).
   - Geneest 30 HP, maar nooit boven je maximum.
   - Tijdens een gevecht kost drinken je beurt: de vijand valt meteen aan.
2. **Vluchten**
   - Kan alleen tijdens een gevecht.
   - Voor de eindbaas kun je niet vluchten (`409 Conflict`).
   - Lukt in 60% van de gevallen. Mislukt het, dan krijgt de vijand een gratis aanval.
3. **Sterker worden**
   - Na iedere verslagen vijand stijgt je aanvalskracht met 1.
   - Een verslagen vijand laat in 25% van de gevallen een drankje vallen (als je tas niet vol is).

Alle getallen staan in `RPG_API/Services/GameRules.cs`, zodat je de balans op één plek kunt aanpassen.

---

## Opbouw

```text
RPG_Console/                    Invoer, menu's, weergave
  Program.cs                    Maakt HttpClient en start ConsoleGame
  ConsoleGame.cs                Menu-lussen, kiest welke API-actie wordt aangeroepen
  ConsoleInput.cs               Invoer lezen en controleren (lege invoer, onbekende keuze)
  ConsoleRenderer.cs            Alles tekenen (panelen, HP-balken, menu's)
  EnemyArt.cs                   ASCII-tekeningen per vijand
  Api/GameApiClient.cs          Alle HTTP-aanroepen, leest foutmeldingen van de API
  Models/GameModels.cs          Kopieën van de DTO's

RPG_API/                        Spelregels en opslag
  Program.cs                    Dependency Injection, middleware, migrations bij start
  Controllers/                  GameController, StatusController, LogsController
  Services/                     Spelregels (zie hieronder)
  Entities/                     Game, Player, Enemy (+ subclasses), ApiRequestLog
  Database/GameDbContext.cs     DbContext + seeding van de vijanden
  Models/                       DTO's (requests en responses) en ErrorResponse
  Middleware/                   Request-logging en foutafhandeling
  Exceptions/                   Eigen exceptions → HTTP-statuscodes
  RPG_API.http                  Alle requests om de API los te testen
```

### Services
| Service | Verantwoordelijkheid |
| --- | --- |
| `GameService` (`IGameService`) | Eén ingang voor de controller, stuurt door naar de juiste service |
| `GameStartService` | Nieuwe speler en spel aanmaken |
| `GameQueryService` | Spellen ophalen |
| `GameStateService` | Laden, controles (`EnsureActive`, `EnsureInCombat`...), opslaan, response maken |
| `MovementService` | Volgend level en wat er in die ruimte gebeurt |
| `CombatGameService` | Aanvallen en vluchten |
| `CombatRoundService` | Eén gevechtsronde: volgorde, schade, overwinning, game over |
| `CombatService` | Schade en snelheid van één aanval |
| `PotionService` | Drankje drinken (zelf uitgewerkte actie) |
| `GameRules` / `DamageCalculator` | Vaste regels en veilige HP-berekening |
| `IRandomGenerator` | Toeval achter een interface |

### Endpoints
| Methode | Route | Doel | Mogelijke fouten |
| --- | --- | --- | --- |
| GET | `/api/status` | Werkt de API? | |
| GET | `/api/info` | Spelinformatie (titel, levels, rustplaatsen...) | |
| POST | `/api/game` | Nieuw spel `{ "playerName": "..." }` → **201** | 400 |
| GET | `/api/game` | Alle opgeslagen spellen | |
| GET | `/api/game/{id}` | Toestand van één spel | 404 |
| POST | `/api/game/{id}/move` | Naar het volgende level | 404, 409 |
| POST | `/api/game/{id}/attack` | `{ "type": "Light" }` of `"Heavy"` | 400, 404, 409 |
| POST | `/api/game/{id}/run` | Vluchten | 404, 409 |
| POST | `/api/game/{id}/potion` | Drankje drinken | 404, 409 |
| GET | `/api/logs?take=20` | Laatste request-logs | |

Iedere fout heeft dezelfde vorm: `{ "status": 409, "error": "Je hebt geen genezingsdrankjes meer.", "path": "/api/game/3/potion" }`.

| Code | Betekenis in dit spel |
| --- | --- |
| 200 OK | Actie gelukt |
| 201 Created | Nieuw spel aangemaakt |
| 400 Bad Request | Ongeldige invoer (lege naam, ongeldige JSON, onbekende aanval) |
| 404 Not Found | Spel bestaat niet |
| 409 Conflict | Invoer klopt, maar de actie mag nu niet (bewegen tijdens gevecht, geen drankjes, vluchten voor de baas) |
| 500 Internal Server Error | Onverwachte fout; details staan alleen in de API-terminal |

---

## Eén actie gevolgd: drankje drinken

1. **Console**: de speler kiest "Drankje drinken". `ConsoleGame` roept `GameApiClient.UsePotionAsync(id)` aan.
2. **HTTP**: `POST /api/game/3/potion` gaat naar de API.
3. **Middleware**: `RequestLoggingMiddleware` start een stopwatch, `ExceptionHandlingMiddleware` staat klaar om fouten af te vangen.
4. **Controller**: `GameController.UsePotion` geeft het id door aan `IGameService`.
5. **Service**: `PotionService.UsePotionAsync`
   - laadt het spel met speler en vijand (`GameStateService.LoadAsync`, anders 404)
   - controleert: spel loopt nog, drankjes > 0, HP niet vol (anders 409)
   - past de toestand aan: HP omhoog (`DamageCalculator.Heal`), drankjes −1
   - in een gevecht: de vijand valt aan (`CombatRoundService.EnemyFreeAttack`)
6. **EF Core**: `SaveAsync` zet `UpdatedAt` en roept `SaveChangesAsync()` aan → UPDATE in MySQL.
7. **Response**: `GameResponseMapper` maakt een `GameResponse`-DTO → JSON terug.
8. **Middleware**: de request wordt opgeslagen in `ApiRequestLogs` (methode, route, status, duur).
9. **Console**: `ConsoleRenderer` tekent de nieuwe HP, het aantal drankjes en het bericht.

---

## Gebruikte voorbeeldcode en wat er is veranderd

Basis: de voorbeeldapplicatie [DINH-iy/Framework_P2](https://github.com/DINH-iy/Framework_P2).

| Onderdeel | Overgenomen uit het voorbeeld | Veranderd of nieuw |
| --- | --- | --- |
| Projectopbouw | Console + Web API, controller → service → EF Core | Namespaces opgeschoond (`RPG_API.*`), dubbele service-registratie verwijderd |
| Entities | `Game`, `Player`, abstracte `Enemy` + subclasses, `ApiRequestLog` | `Potions`, `EnemiesDefeated`, `CreatedAt`/`UpdatedAt`, `Description`, `IsBoss`; nieuwe vijand `BellWarden`; eigen stats |
| Seeding | `HasData` per vijand-subclass | Vierde vijand (eindbaas) |
| Levels | Tien levels, rustplaatsen, uitgang | Eigen namen per level, rust op 5 en 9, schatkamers, eindbaas op 10 met daarachter de uitgang |
| Spelinformatie | — | `GET /api/info` met `InfoController` + `IGameInfoService` (week 2/3) |
| Encapsulatie | Public setters op `Player` | `private set` + methodes `TakeDamage`, `Heal`, `UsePotion`, ... (week 6) |
| Gevecht | `CombatService` (licht/zwaar), snelheid bepaalt volgorde | Schade via `DamageCalculator`, beloning na overwinning, berichten in het Nederlands |
| Vluchten | Endpoint `run` | Eigen regel: 60% kans, mislukken = gratis aanval, niet bij de baas |
| Drankje | — | **Volledig zelf uitgewerkte spelactie** (`PotionService`, endpoint `potion`) |
| Foutafhandeling | Console toonde de ruwe foutmelding | Eigen exceptions, `ExceptionHandlingMiddleware` met 400/404/409/500, eigen vorm voor validatiefouten |
| Request-logging | `RequestLoggingMiddleware` | Logt nu ook de juiste statuscode bij afgevangen fouten, eigen DbContext-scope, endpoint `/api/logs` |
| DTO's / mapping | `GameResponse`, `GameResponseMapper` | Validatie met `[Required]`, `[StringLength]`, `[EnumDataType]` |
| Interfaces | — | `IGameService`, `IRandomGenerator` |
| Console | Menu's, `GameApiClient`, renderer | Nederlandse teksten, kleur, HP-balken, ASCII per vijand, `ApiException`, offline-melding, alleen lopende spellen hervatten |

---

## Meer uitleg
- [docs/WEKEN.md](docs/WEKEN.md): per week (1 t/m 7) de Canvas-opdracht en waar die in de code zit
- [docs/TAKEN.md](docs/TAKEN.md): taakverdeling console / controller / service (week 3)
- [docs/ERD.md](docs/ERD.md): relaties tussen de tabellen (week 4 en 6)
- [docs/LEERDOELEN.md](docs/LEERDOELEN.md): per leerdoel waar het in de code zit
- [docs/DEBUGGEN.md](docs/DEBUGGEN.md): de debug-oefening en hoe je een aanval volgt
