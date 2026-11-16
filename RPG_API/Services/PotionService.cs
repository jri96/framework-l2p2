using RPG_API.Exceptions;
using RPG_API.Models;

namespace RPG_API.Services;

// ZELF UITGEWERKTE SPELACTIE: een genezingsdrankje drinken.
// Stappen: request ontvangen (controller) -> spel laden -> voorwaarden controleren
// -> toestand aanpassen -> opslaan -> response maken.
public class PotionService
{
    private readonly GameStateService _gameState;
    private readonly CombatRoundService _combatRound;

    public PotionService(GameStateService gameState, CombatRoundService combatRound)
    {
        _gameState = gameState;
        _combatRound = combatRound;
    }

    public async Task<GameResponse> UsePotionAsync(int gameId)
    {
        // 1. Spel laden (404 als het niet bestaat)
        var game = await _gameState.LoadAsync(gameId);
        var player = game.Player;

        // 2. Voorwaarden controleren (409 als de actie nu niet mag)
        _gameState.EnsureActive(game);

        if (player.Potions <= 0)
            throw new GameRuleException("Je hebt geen genezingsdrankjes meer.");

        if (player.HasFullHealth)
            throw new GameRuleException("Je bent al volledig gezond. Bewaar je drankje voor later.");

        // 3. Toestand aanpassen
        player.UsePotion();
        var healed = player.Heal(GameRules.PotionHealAmount);

        var message = $"Je drinkt een drankje en geneest {healed} HP. " +
                      $"Nog {player.Potions} drankje(s) over. ";

        // Tijdens een gevecht kost drinken je beurt: de vijand valt aan.
        CombatRoundResult? round = null;
        if (game.CurrentEnemy is not null)
        {
            round = _combatRound.EnemyFreeAttack(game);
            message += round.Message;
        }

        // 4. Opslaan en 5. response maken
        await _gameState.SaveAsync(game);
        return _gameState.CreateResponse(game, message, round);
    }
}
