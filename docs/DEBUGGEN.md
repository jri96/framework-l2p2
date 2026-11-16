# Debuggen

## Starten met de debugger
- VS Code → *Run and Debug* → **API + Console** → F5.
- Alleen `dotnet run` koppelt geen debugger. Gebruik de debug-configuratie of *Attach to process*.
- F5 = doorgaan, F10 = step over, F11 = step into, Shift+F11 = step out.

## Oefening van Canvas: `ApplyDamage`

```csharp
static int ApplyDamage(int health, int damage)
{
    health -= damage;

    if (health < 0)
    {
        health = 100;
    }

    return health;
}
```

**Voorspelling** met `health = 10` en `damage = 15`: je verwacht 0.

**Stappen met de debugger** (breakpoint op `health -= damage;`):
| Na regel | `health` |
| --- | --- |
| begin | 10 |
| `health -= damage;` | -5 |
| `if (health < 0)` | voorwaarde is waar |
| `health = 100;` | **100** ← hier gaat het fout |

De fout: bij te veel schade wordt de speler volledig genezen in plaats van op 0 gezet.

**Oplossing** (staat in `RPG_API/Services/DamageCalculator.cs` en wordt in het spel gebruikt):

```csharp
public static int ApplyDamage(int health, int damage)
{
    if (damage < 0)
        damage = 0;

    return Math.Max(0, health - damage);
}
```

**Controle**
| health | damage | resultaat |
| --- | --- | --- |
| 10 | 15 | 0 |
| 10 | 0 | 10 |
| 10 | 10 | 0 |
| 10 | -5 | 10 (negatieve schade geneest niet) |

## Een aanval volgen van controller naar service
1. Breakpoint in `GameController.Attack` en in `CombatRoundService.Resolve`.
2. Start **API + Console**, ga naar een gevecht en kies "Zware aanval".
3. In de controller: controleer `id` en `request.Type` (moet `Heavy` zijn).
4. F5 → je stopt in `Resolve`. Bekijk in *Variables* of *Watch*:
   - `game.Player.Health`, `game.CurrentEnemyHealth`
   - `playerAttack.Speed` en `enemyAttack.Speed`: welke tak van de `if` wordt gekozen?
5. F10 over `ApplyPlayerAttack(...)` → `game.CurrentEnemyHealth` daalt.
6. Bekijk de *Call Stack*: `GameController.Attack` → `GameService.AttackAsync` → `CombatGameService.AttackAsync` → `CombatRoundService.Resolve`.

**Welke controle houdt een ongeldige actie tegen?** Stuur via `RPG_API.http` een aanval terwijl er geen vijand is (`POST /api/game/1/attack`). Je komt in `GameStateService.EnsureInCombat` en ziet de `GameRuleException` ontstaan. `ExceptionHandlingMiddleware` maakt er een 409 van.

## Exceptions onderzoeken
- Laat de debugger stoppen bij iedere exception: *Breakpoints* → vink **All Exceptions** of **User-Unhandled Exceptions** aan.
- Lees het fouttype, de melding en de call stack. Kijk naar de eerste regel die in je eigen code staat.
- Bekijk de tabel `ApiRequestLogs` of `GET /api/logs`: daar staan statuscode en foutmelding per request.
