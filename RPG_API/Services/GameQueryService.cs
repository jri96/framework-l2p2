using Microsoft.EntityFrameworkCore;
using RPG_API.Database;
using RPG_API.Models;

namespace RPG_API.Services;

public class GameQueryService
{
    private readonly GameDbContext _context;
    private readonly GameStateService _gameState;

    public GameQueryService(GameDbContext context, GameStateService gameState)
    {
        _context = context;
        _gameState = gameState;
    }

    public async Task<List<GameSummaryResponse>> GetGamesAsync()
    {
        var games = await _context.Games
            .Include(game => game.Player)
            .OrderByDescending(game => game.UpdatedAt)
            .ToListAsync();

        var summaries = new List<GameSummaryResponse>();

        foreach (var game in games)
        {
            summaries.Add(new GameSummaryResponse
            {
                Id = game.Id,
                PlayerName = game.Player.Name,
                CurrentLevel = game.CurrentLevel,
                CurrentLocation = game.CurrentLocation,
                Health = game.Player.Health,
                MaxHealth = game.Player.MaxHealth,
                IsFinished = game.IsFinished,
                IsWon = game.IsWon,
                UpdatedAt = game.UpdatedAt
            });
        }

        return summaries;
    }

    public async Task<GameResponse> GetAsync(int gameId)
    {
        var game = await _gameState.LoadAsync(gameId);
        return _gameState.CreateResponse(game);
    }
}
