using RPG_API.Database;
using RPG_API.Entities;
using RPG_API.Enums;
using RPG_API.Exceptions;
using RPG_API.Models;

namespace RPG_API.Services;

public class GameStartService
{
    private readonly GameDbContext _context;
    private readonly GameStateService _gameState;

    public GameStartService(GameDbContext context, GameStateService gameState)
    {
        _context = context;
        _gameState = gameState;
    }

    public async Task<GameResponse> StartAsync(StartGameRequest request)
    {
        var playerName = request.PlayerName.Trim();

        // Extra controle naast [Required]: een naam met alleen spaties is ook leeg.
        if (string.IsNullOrWhiteSpace(playerName))
            throw new InvalidInputException("Vul een spelernaam in.");

        var player = new Player { Name = playerName };

        var game = new Game
        {
            Player = player,
            CurrentLevel = 1,
            CurrentLocation = GameRules.GetLocationName(1),
            CurrentRoomType = RoomType.Start
        };

        _context.Games.Add(game);
        await _gameState.SaveAsync(game);

        var message = $"Welkom, {player.Name}. Je staat bij de {game.CurrentLocation}. " +
                      $"Bereik level {GameRules.MaxLevel} om de uitgang te vinden.";

        var savedGame = await _gameState.LoadAsync(game.Id);
        return _gameState.CreateResponse(savedGame, message);
    }
}
