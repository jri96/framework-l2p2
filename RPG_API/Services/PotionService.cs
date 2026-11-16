using RPG_API.Exceptions;
using RPG_API.Models;

namespace RPG_API.Services;

// ZELF UITGEWERKTE SPELREGEL: een genezingsdrankje drinken.
// Voorwaarden (vooraf opgeschreven):
// - het spel loopt nog;
// - de speler heeft minstens één drankje;
// - de speler heeft geen volle HP (anders wordt de actie geweigerd);
// - genezen gaat nooit boven de maximale HP.
public class PotionService
{
    private readonly GameStateService _gameState;

    public PotionService(GameStateService gameState)
    {
        _gameState = gameState;
    }

    public async Task<GameResponse> UsePotionAsync(int gameId)
    {
        // 1. Spel laden
        var game = await _gameState.LoadAsync(gameId);
        var player = game.Player;

        // 2. Voorwaarden controleren
        _gameState.EnsureActive(game);

        if (player.Potions <= 0)
            throw new GameRuleException("Je hebt geen genezingsdrankjes meer.");

        if (player.Health >= player.MaxHealth)
            throw new GameRuleException("Je bent al volledig gezond. Bewaar je drankje voor later.");

        // 3. Toestand aanpassen
        var oldHealth = player.Health;
        player.Health = DamageCalculator.Heal(player.Health, GameRules.PotionHealAmount, player.MaxHealth);
        player.Potions--;

        var message = $"Je drinkt een drankje en geneest {player.Health - oldHealth} HP. " +
                      $"Nog {player.Potions} drankje(s) over.";

        // 4. Opslaan en 5. response maken
        await _gameState.SaveAsync(game);
        return _gameState.CreateResponse(game, message);
    }
}
