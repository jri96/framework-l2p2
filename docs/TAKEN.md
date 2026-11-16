# Taakverdeling (week 3)

Welke taken heeft het spel nodig, en wat doet de console, de controller en de service bij iedere taak?

| Taak | Console | Controller | Service |
| --- | --- | --- | --- |
| API-status tonen | `GET /api/status`, toont online/offline | `StatusController.Get` | `StatusService.GetMessage` |
| Spelinformatie tonen | `GET /api/info`, toont titel, omschrijving en levels | `InfoController.Get` | `GameInfoService.GetInfo` |
| Spel starten | vraagt naam, `POST /api/game` | ontvangt `StartGameRequest`, geeft 201 terug | `GameStartService`: speler + spel aanmaken en opslaan |
| Spel hervatten | `GET /api/game`, laat kiezen, `GET /api/game/{id}` | `GetGames`, `Get` | `GameQueryService`: spellen ophalen |
| Bewegen | keuze "verder", `POST /api/game/{id}/move` | `Move` | `MovementService`: volgend level, ruimte bepalen, rust, uitgang |
| Aanvallen | keuze licht/zwaar, `POST /api/game/{id}/attack` | `Attack` | `CombatGameService` + `CombatRoundService` + `CombatService` |
| Vluchten | keuze "vluchten", `POST /api/game/{id}/run` | `Run` | `CombatGameService.RunAsync` |
| Drankje drinken | keuze "drankje", `POST /api/game/{id}/potion` | `UsePotion` | `PotionService` |

## Hoe verdeel ik de services?
- **Eén service per soort actie.** Bewegen, vechten en drankjes veranderen elk een ander deel van de spelstatus.
- **Gedeelde stappen apart.** Spel laden, controleren of het nog loopt, opslaan en een response maken komen bij iedere actie terug. Die komen in `GameStateService`, zodat ze niet in elke service herhaald worden.
- **Rekenwerk los van opslaan.** `CombatService` rekent alleen schade en snelheid uit en weet niets van de database. Daardoor is hij makkelijk te begrijpen en te testen.
- **De controller blijft dun.** Hij ontvangt het request en geeft het resultaat terug. Spelregels staan nooit in de controller en nooit in de console.
