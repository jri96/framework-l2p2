using RPG_API.Database;
using RPG_API.Entities;
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
        var player = new Player { Name = request.PlayerName.Trim() };

        var game = new Game
        {
            Player = player,
            CurrentLevel = 1,
            CurrentLocation = GameRules.GetLocationName(1)
        };

        _context.Games.Add(game);
        await _gameState.SaveAsync(game);

        var message = $"Welkom, {player.Name}. Je staat bij de {game.CurrentLocation}. " +
                      "Hoog boven je luidt de Klokkenwachter.";

        var savedGame = await _gameState.LoadAsync(game.Id);
        return _gameState.CreateResponse(savedGame, message);
    }
}
