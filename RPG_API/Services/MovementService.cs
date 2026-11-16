using Microsoft.EntityFrameworkCore;
using RPG_API.Database;
using RPG_API.Entities;
using RPG_API.Enums;
using RPG_API.Models;

namespace RPG_API.Services;

// Bepaalt wat er gebeurt als de speler naar het volgende level gaat.
public class MovementService
{
    private readonly GameDbContext _context;
    private readonly GameStateService _gameState;
    private readonly IRandomGenerator _random;

    public MovementService(GameDbContext context, GameStateService gameState, IRandomGenerator random)
    {
        _context = context;
        _gameState = gameState;
        _random = random;
    }

    public async Task<GameResponse> MoveAsync(int gameId)
    {
        var game = await _gameState.LoadAsync(gameId);
        _gameState.EnsureActive(game);
        _gameState.EnsureNotInCombat(game);

        game.CurrentLevel++;
        game.CurrentLocation = GameRules.GetLocationName(game.CurrentLevel);
        var message = $"Je loopt verder naar level {game.CurrentLevel}: {game.CurrentLocation}. ";

        if (game.CurrentLevel == GameRules.BossLevel)
            message += await EnterBossRoomAsync(game);
        else if (GameRules.IsRestLevel(game.CurrentLevel))
            message += EnterRestRoom(game);
        else
            message += await EnterRandomRoomAsync(game);

        await _gameState.SaveAsync(game);
        return _gameState.CreateResponse(game, message);
    }

    private async Task<string> EnterBossRoomAsync(Game game)
    {
        var boss = await _context.Enemies.FirstAsync(enemy => enemy.IsBoss);

        game.CurrentRoomType = RoomType.Boss;
        game.CurrentEnemy = boss;
        game.CurrentEnemyId = boss.Id;
        game.CurrentEnemyHealth = boss.MaxHealth;

        return $"De klokken luiden oorverdovend. De {boss.Name} komt op je af! Vluchten kan hier niet.";
    }

    private static string EnterRestRoom(Game game)
    {
        game.CurrentRoomType = RoomType.Rest;
        game.Player.RestoreFullHealth();

        return "Je vindt een rustplaats bij een kaars. Je bent volledig genezen.";
    }

    private async Task<string> EnterRandomRoomAsync(Game game)
    {
        var roomType = GameRules.ChooseRandomRoom(_random.Next(0, 100));
        game.CurrentRoomType = roomType;

        switch (roomType)
        {
            case RoomType.Combat:
                var enemies = await _context.Enemies
                    .Where(enemy => !enemy.IsBoss)
                    .ToListAsync();

                var enemyToFight = enemies[_random.Next(0, enemies.Count)];
                game.CurrentEnemy = enemyToFight;
                game.CurrentEnemyId = enemyToFight.Id;
                game.CurrentEnemyHealth = enemyToFight.MaxHealth;
                return $"Een {enemyToFight.Name} verspert de weg!";

            case RoomType.Treasure:
                return OpenTreasure(game.Player);

            default:
                return "De ruimte is leeg. Alleen het geluid van druppelend water.";
        }
    }

    private static string OpenTreasure(Player player)
    {
        if (player.TryAddPotion())
            return $"Je vindt een schatkist met een genezingsdrankje. Je hebt er nu {player.Potions}.";

        player.Heal(15);
        return "Je vindt een schatkist, maar je tas zit vol. Je eet het brood dat erin ligt (+15 HP).";
    }
}
