# Leerdoelen: waar zit het in de code?

Voorbereiding voor het beoordelingsgesprek (week 8 en 9). Per leerdoel staat waar je het in dit project laat zien, met een korte uitleg in je eigen woorden.

| Leerdoel | Waar in de code | Uitleg |
| --- | --- | --- |
| Wat C# en .NET zijn | Hele solution | C# is de programmeertaal. .NET is het platform (runtime + bibliotheken + SDK) waarop de code draait. ASP.NET Core en EF Core zijn onderdelen van .NET. |
| Ontwikkelomgeving opzetten | `setup.ps1`, `.vscode/`, `dotnet-tools.json` | .NET 10 SDK, C# Dev Kit, `dotnet-ef` en de NuGet-packages uit `RPG_API.csproj`. |
| Solution met meerdere projecten | `DungeonCrawler.slnx` | Twee projecten: `RPG_Console` en `RPG_API`. Het zijn twee losse programma's die apart starten. |
| Console-applicatie | `RPG_Console/` | `while`-lus met menu, `Console.ReadLine()`, `switch` op de keuze. |
| ASP.NET Core Web API | `RPG_API/Program.cs`, `Controllers/` | `WebApplication.CreateBuilder`, `AddControllers`, `MapControllers`, attributes `[HttpGet]`/`[HttpPost]`. |
| Client-serverarchitectuur | `GameApiClient` ↔ `GameController` | De console (client) stuurt requests, de API (server) verwerkt ze en stuurt een response. De client heeft geen toegang tot de database. |
| HTTP en JSON | `GameApiClient.cs`, `RPG_API.http` | `GetAsync`, `PostAsJsonAsync`, `ReadFromJsonAsync`. Body en response zijn JSON. |
| Waar business logic thuishoort | `Services/` | Niet in de console en niet in de controller, maar in de services. De controller geeft alleen door. |
| Business logic in services | `MovementService`, `CombatRoundService`, `PotionService` | Iedere service heeft één taak. |
| Scheiden van verantwoordelijkheden | Zie tabel "Services" in de README | Kleine onderdelen zijn makkelijker te testen, te begrijpen en te veranderen. Een andere UI (bijv. een website) kan dezelfde API gebruiken. |
| Dependency Injection | `Program.cs` (`AddScoped`, `AddSingleton`), constructors van services | Een class vraagt in zijn constructor wat hij nodig heeft; de DI-container maakt die objecten aan. Scoped = één per request (zelfde DbContext in alle services), Singleton = één voor de hele app. |
| EF Core en de database | `GameDbContext`, `Services/*` | EF Core is een ORM: C#-objecten worden rijen in MySQL. LINQ (`Where`, `FirstOrDefaultAsync`) wordt SQL. |
| DbContext en entities | `Database/GameDbContext.cs`, `Entities/` | Iedere `DbSet` is een tabel, iedere entity-class een rij. |
| Migrations | `RPG_API/Migrations/` (gemaakt met `dotnet ef migrations add InitialGame`) | Een migration beschrijft een wijziging aan de database in C#. `database update` voert hem uit. |
| Seeding | `OnModelCreating` → `HasData` | De vier vijanden staan na de migration direct in de tabel `Enemies`. |
| Relaties | `Game.Player`, `Game.CurrentEnemy` | Een spel hoort bij één speler (verplicht) en heeft optioneel een huidige vijand (`int?`). `Include()` laadt de gekoppelde gegevens mee. |
| State opslaan en ophalen | `Game` + `Player`, `GameStateService.LoadAsync`/`SaveAsync` | Na iedere actie staat alles in MySQL. Daardoor kun je de console sluiten en later verder spelen. |
| async, await, Task | Alle services, controllers, `GameApiClient` | Wachten op de database of het netwerk blokkeert de thread niet. `Task<T>` = een resultaat dat later komt. |
| OOP: classes, inheritance, encapsulatie | `Enemy` (abstract) → `Skeleton`, `Goblin`, `Orc`, `BellWarden`; `AttackResult` met alleen getters | Subclasses erven de properties en vullen hun eigen waarden in. `private` velden en methodes in services verbergen de werking. |
| Interfaces | `IGameService`, `IRandomGenerator` | De controller hangt af van "iets dat IGameService doet", niet van de concrete class. Handig om te vervangen, bijvoorbeeld een voorspelbare random-generator in een test. |
| DTO's | `Models/GameDtos.cs` en `RPG_Console/Models/GameModels.cs` | De API stuurt nooit entities naar buiten. Een DTO bevat alleen wat de console nodig heeft. |
| Middleware en de request-pipeline | `Program.Configure`, `Middleware/` | Een request gaat langs: RequestLogging → ExceptionHandling → Controller, en de response gaat dezelfde weg terug. |
| Eigen middleware (logging) | `RequestLoggingMiddleware` | Meet de duur en slaat methode, route, statuscode en eventuele foutmelding op in `ApiRequestLogs`. |
| Exception handling | `Exceptions/`, `ExceptionHandlingMiddleware`, `try/catch` in `ConsoleGame` | Services gooien een exception, de middleware maakt er een nette JSON-fout van, de console toont de melding zonder te crashen. |
| HTTP-statuscodes | Tabel in de README | 200, 201, 400, 404, 409 en 500 worden bewust gebruikt. |
| Programmeerlogica | `GameRules`, `CombatRoundService.Resolve`, `ConsoleInput.ReadOption` | Condities (wie is sneller), loops (menu, invoer), enums (`RoomType`, `AttackType`), methodes. |
| Stappen bij het bouwen | README "Starten" en "Opbouw" | Projecten → entities/DbContext → migration → controller → services → console. |
| Verantwoordelijkheden per laag | README "Eén actie gevolgd" | Console = invoer/weergave, controller = HTTP, service = regels, EF Core = opslag. |
| Console ↔ Web API ↔ database | Het hele spel | Zie de keten in de README. |

## Vragen om jezelf te overhoren
- Waarom roept de console nooit de database aan?
- Wat gebeurt er als je `AddScoped<PotionService>()` weghaalt? (Een foutmelding: de DI-container kan `GameService` niet maken, omdat `PotionService` niet geregistreerd is.)
- Waarom staat `RequestLoggingMiddleware` vóór `ExceptionHandlingMiddleware`? (Zo logt hij de echte statuscode 404/409 in plaats van 500.)
- Waarom 409 en niet 400 als je geen drankjes hebt? (De invoer is goed, maar de toestand van het spel laat het niet toe.)
- Waar bewaar je de HP van de vijand, en waarom niet in de tabel `Enemies`? (In `Game.CurrentEnemyHealth`: de tabel `Enemies` bevat de vaste stats die alle spellen delen.)
- Wat doet `Include()` en wat gaat er mis zonder? (Zonder Include en zonder lazy loading is `game.Player` leeg.)
