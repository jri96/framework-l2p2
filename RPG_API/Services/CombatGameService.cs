using RPG_API.Enums;
using RPG_API.Exceptions;
using RPG_API.Models;

namespace RPG_API.Services;

// Spelacties tijdens een gevecht: aanvallen en vluchten.
public class CombatGameService
{
    private readonly GameStateService _gameState;
    private readonly CombatRoundService _combatRound;
    private readonly IRandomGenerator _random;

    public CombatGameService(GameStateService gameState, CombatRoundService combatRound, IRandomGenerator random)
    {
        _gameState = gameState;
        _combatRound = combatRound;
        _random = random;
    }

    public async Task<GameResponse> AttackAsync(int gameId, AttackType attackType)
    {
        var game = await _gameState.LoadAsync(gameId);
        _gameState.EnsureActive(game);
        _gameState.EnsureInCombat(game);

        var round = _combatRound.Resolve(game, attackType);

        await _gameState.SaveAsync(game);
        return _gameState.CreateResponse(game, round.Message, round);
    }

    // Eigen spelregel "vluchten":
    // - Vluchten kan alleen tijdens een gevecht.
    // - Voor de eindbaas kun je niet vluchten.
    // - Vluchten lukt in 60% van de gevallen. Mislukt het, dan krijg je een gratis aanval van de vijand.
    public async Task<GameResponse> RunAsync(int gameId)
    {
        var game = await _gameState.LoadAsync(gameId);
        _gameState.EnsureActive(game);
        _gameState.EnsureInCombat(game);

        var enemy = game.CurrentEnemy!;

        if (enemy.IsBoss)
            throw new GameRuleException($"Je kunt niet vluchten voor de {enemy.Name}. Dit gevecht moet je winnen.");

        if (_random.Chance(GameRules.RunSuccessChance))
        {
            _gameState.ClearEnemy(game);
            game.CurrentRoomType = RoomType.Normal;

            await _gameState.SaveAsync(game);
            return _gameState.CreateResponse(game, $"Je ontsnapt aan de {enemy.Name}. Je kunt verder lopen.");
        }

        var round = _combatRound.EnemyFreeAttack(game);
        var message = $"Je probeert te vluchten, maar de {enemy.Name} houdt je tegen. " + round.Message;

        await _gameState.SaveAsync(game);
        return _gameState.CreateResponse(game, message, round);
    }
}
