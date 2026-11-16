using RPG_API.Entities;
using RPG_API.Enums;
using RPG_API.Models;

namespace RPG_API.Services;

// Bepaalt wat er gebeurt als de speler naar het volgende level gaat.
public class MovementService
{
    private readonly GameStateService _gameState;
    private readonly IRandomGenerator _random;

    public MovementService(GameStateService gameState, IRandomGenerator random)
    {
        _gameState = gameState;
        _random = random;
    }

    public async Task<GameResponse> MoveAsync(int gameId)
    {
        // 1. Haal de game op.
        var game = await _gameState.LoadAsync(gameId);

        // 2. Controleer of de actie mag.
        _gameState.EnsureActive(game);

        // 3. Bepaal het volgende level en de ruimte.
        game.CurrentLevel++;
        game.CurrentLocation = GameRules.GetLocationName(game.CurrentLevel);
        var message = $"Je loopt verder naar level {game.CurrentLevel}: {game.CurrentLocation}. ";

        if (game.CurrentLevel == GameRules.MaxLevel)
            message += EnterExit(game);
        else if (GameRules.IsRestLevel(game.CurrentLevel))
            message += EnterRestRoom(game);
        else
            message += EnterRandomRoom(game);

        // 4. Pas de toestand aan en sla deze op.
        await _gameState.SaveAsync(game);

        // 5. Geef een response-DTO terug.
        return _gameState.CreateResponse(game, message);
    }

    private static string EnterExit(Game game)
    {
        game.CurrentRoomType = RoomType.Exit;
        game.IsFinished = true;
        game.IsWon = true;

        return "Je ziet daglicht door een deur. Je hebt de uitgang bereikt en gewonnen!";
    }

    private static string EnterRestRoom(Game game)
    {
        game.CurrentRoomType = RoomType.Rest;
        game.Player.Health = game.Player.MaxHealth;

        return "Je vindt een rustplaats bij een kaars. Je bent volledig genezen.";
    }

    private string EnterRandomRoom(Game game)
    {
        var roomType = GameRules.ChooseRandomRoom(_random.Next(0, 100));
        game.CurrentRoomType = roomType;

        if (roomType == RoomType.Treasure)
            return OpenTreasure(game.Player);

        return "De ruimte is leeg. Alleen het geluid van druppelend water.";
    }

    private static string OpenTreasure(Player player)
    {
        if (player.Potions < GameRules.MaxPotions)
        {
            player.Potions++;
            return $"Je vindt een schatkist met een genezingsdrankje. Je hebt er nu {player.Potions}.";
        }

        player.Health = DamageCalculator.Heal(player.Health, 15, player.MaxHealth);
        return "Je vindt een schatkist, maar je tas zit vol. Je eet het brood dat erin ligt (+15 HP).";
    }
}
