# Weekoverzicht periode 2 (week 1 t/m 7)

Per week: wat de Canvas-opdracht vraagt, waar het in de code staat, wat je laat zien en wat je moet kunnen uitleggen.
In Git staat per week een commit (`Week 1: ...` t/m `Week 7: ...`). Met `git log --oneline` zie je de opbouw, en met `git checkout <commit>` kun je een eerdere week bekijken.
Week 8 en 9 zijn de beoordelingsgesprekken; in week 10 is er geen les.

---

## Week 1: projecten starten
**Canvas:** solution met `RPG_Console` en `RPG_API`; endpoint `/api/status`; menu met *Nieuw spel* en *Afsluiten*; naam vragen, welkomstbericht tonen; lege naam en onbekende keuze afhandelen; eigen titel en teksten; opslaan in Git.

| Onderdeel | Waar |
| --- | --- |
| Solution | `DungeonCrawler.slnx` |
| Status-endpoint | week 1: `MapGet` in `Program.cs` → nu `StatusController` |
| Menu-lus | `ConsoleGame.RunMainMenuAsync` |
| Lege invoer / onbekende keuze | `ConsoleInput.ReadRequired`, `ReadOption` |

**Uitleggen:** wat C# en .NET zijn; console en API zijn twee losse programma's; welke taak ieder project heeft.

---

## Week 2: HTTP-communicatie
**Canvas:** `HttpClient` in de console; status ophalen met `GetFromJsonAsync`; een DTO; een **tweede GET-endpoint met spelinformatie** dat de console toont; een waarde in de API veranderen en zien dat de console meeverandert; de API stoppen en kijken wat er gebeurt.

| Onderdeel | Waar |
| --- | --- |
| Spelinformatie | `GET /api/info` → `GameInfoResponse` (titel, omschrijving, levels, rustplaatsen, eindbaas, start-HP, drankjes) |
| Waarden | `GameRules.Title`, `Description`, `MaxLevel`, ... |
| HttpClient | `RPG_Console/Program.cs`, `GameApiClient.GetGameInfoAsync` |
| Tonen | hoofdmenu → *3. Spelinformatie* (`ConsoleRenderer.ShowInfo`) |
| API gestopt | hoofdmenu toont "API: offline"; acties tonen een melding in plaats van te crashen |

**Laten zien:** verander `GameRules.Title`, herstart de API en open *Spelinformatie*.
**Uitleggen:** wijs bij één request de methode (`GET`), URL (`/api/info`), statuscode (`200`) en JSON aan. De console is de client, de API is de server.

---

## Week 3: API-structuur
**Canvas:** endpoints naar controllers; services achter een interface; registreren met `AddScoped`; injecteren via de constructor; hetzelfde voor de spelinformatie; requestpad en DTO-velden gelijk houden; **per taak opschrijven wat console, controller en service doen**.

| Onderdeel | Waar |
| --- | --- |
| Controllers | `StatusController`, `InfoController`, `GameController` |
| Services + interfaces | `IStatusService`, `IGameInfoService`, `IGameService` |
| Registratie | `Program.ConfigureServices` |
| Taakverdeling | [TAKEN.md](TAKEN.md) |

**Uitleggen:** waarom de controller de service niet met `new` maakt; `AddScoped` tegenover `AddSingleton`; hoe je de services hebt verdeeld.

---

## Week 4: database en spel starten
**Canvas:** entities `Player` en `Game`; `GameDbContext`; EF Core- en MySQL-packages; migration `InitialGame`; `POST /api/game` slaat speler en game op en geeft een DTO met het id terug; ophalen met `Include`; **relatie Player-Game tekenen**; na herstart hetzelfde spel ophalen; migrations in Git.

| Onderdeel | Waar |
| --- | --- |
| Entities | `Entities/Player.cs`, `Entities/Game.cs` |
| DbContext | `Database/GameDbContext.cs` |
| Migration | `RPG_API/Migrations` (`dotnet ef migrations add InitialGame ...`, gebeurt in `start.bat`) |
| Starten / ophalen | `GameStartService`, `GameQueryService`, `GameStateService.LoadAsync` |
| Relatie + gekozen velden | [ERD.md](ERD.md) |

**Laten zien:** start een spel, sluit beide programma's, start opnieuw en kies *Spel hervatten*. Bekijk de tabellen in phpMyAdmin.
**Let op:** de migration wordt op je laptop gemaakt. Commit daarna de map `RPG_API/Migrations` (`git add RPG_API/Migrations`, `git commit -m "Migration InitialGame"`, `git push`).

---

## Week 5: bewegen, rustplaats en uitgang
**Canvas:** `POST /api/game/{id}/move`; vaste volgorde in de service (ophalen → controleren → level en ruimte → opslaan → DTO); enum voor ruimtes; rustplaats; uitgang en eindstatus; afgelopen spel nog bekijken maar geen acties; tien levels; **minstens één eigen spelregel met vooraf opgeschreven voorwaarden**; testen met een onbekende game en een actie na het einde.

| Onderdeel | Waar |
| --- | --- |
| Bewegen | `MovementService.MoveAsync` (de vijf stappen staan als commentaar) |
| Ruimtes | `Enums/RoomType.cs`: `Start`, `Normal`, `Combat`, `Treasure`, `Rest`, `Boss`, `Exit` |
| Rust / uitgang | rust op level 5 en 9; uitgang na de eindbaas op level 10 |
| Geen acties na het einde | `GameStateService.EnsureActive` |
| Eigen spelregel | genezingsdrankje (`PotionService`), voorwaarden in de README |
| Afgelopen spel bekijken | hoofdmenu → *Spel hervatten of bekijken* |

---

## Week 6: vijanden en gevechten
**Canvas:** `Enemy` + seeding + relatie + migration; actuele vijand-HP bij de game; `POST /api/game/{id}/attack`; snelste eerst; direct controleren of iemand verslagen is; verslagen vijand slaat niet terug; bij 0 HP game over; niet bewegen tijdens een gevecht; **encapsulatie** (gezondheid via methodes); eigen vijandnamen, sterktes en berichten; de helper gebruiken of bewust één regel aanpassen.

| Onderdeel | Waar |
| --- | --- |
| Vijanden + seeding | `Entities/Enemy.cs`, `Entities/Enemies/*.cs`, `GameDbContext.OnModelCreating` |
| Vijand-HP per spel | `Game.CurrentEnemyHealth` |
| Beurt | `CombatRoundService.Resolve` |
| Aanval / helper | `CombatService.PerformAttack`, `DamageCalculator` |
| Encapsulatie | `Player`: `private set` + `TakeDamage`, `Heal`, `RestoreFullHealth`, `TryAddPotion`, `UsePotion` |
| Niet bewegen in gevecht | `GameStateService.EnsureNotInCombat` |
| Aangepaste regel | `DamageCalculator.ApplyDamage`: nooit onder 0, negatieve schade telt niet |

**Testen (Canvas):**
- speler slaat eerst: val een Ork aan (snelheid 6);
- vijand slaat eerst: val een Kobold aan met een zware aanval (snelheid 16 tegen 10);
- iemand gaat direct neer: een vijand met weinig HP over;
- hervatten tijdens een gevecht: hoofdmenu → hervatten → zelfde vijand en HP.

---

## Week 7: fouten en logging
**Canvas:** statuscodes 400 / 404 / 409 / 500; in de console eerst de statuscode controleren; `HttpRequestException` afvangen; request-logging-middleware met methode, pad, status en duur; de volgorde in `Program.cs` zo dat de logging de uiteindelijke status ziet; testen met onbekend id, ongeldige invoer, bewegen tijdens een gevecht en een gestopte API; **een geweigerde actie verandert de opgeslagen game niet**; README met startinstructies en gebruikte code; één actie voorbereiden van console tot database.

| Onderdeel | Waar |
| --- | --- |
| Foutafhandeling | `Middleware/ExceptionHandlingMiddleware.cs`, `Exceptions/GameExceptions.cs` |
| Validatie | `[Required]`, `[StringLength]`, `[EnumDataType]` + `InvalidModelStateResponseFactory` |
| Logging | `Middleware/RequestLoggingMiddleware.cs` → tabel `ApiRequestLogs`, `GET /api/logs` |
| Volgorde | `Program.Configure`: logging eerst, daarna foutafhandeling |
| Console | `GameApiClient.ReadAsync` (status eerst), `ApiException`, `try/catch` in `ConsoleGame` |
| Geweigerde actie verandert niets | alle controles staan vóór de wijziging en vóór `SaveAsync` |
| Testen | onderaan `RPG_API/RPG_API.http` |
| Eén actie gevolgd | README → "Eén actie gevolgd: drankje drinken" |

---

## Week 8 en 9: beoordelingsgesprek
- Start: WAMP aan → `start.bat`.
- Volg één actie van console tot database (README).
- Laat je eigen spelregels en voorwaarden zien.
- Debug live: [DEBUGGEN.md](DEBUGGEN.md).
- Leerdoelen per onderdeel: [LEERDOELEN.md](LEERDOELEN.md).
