using Microsoft.EntityFrameworkCore;
using RPG_API.Database;
using RPG_API.Entities;
using RPG_API.Models;

namespace RPG_API.Services;

// Gedeelde stappen: spel laden, opslaan en een response maken.
public class GameStateService
{
    private readonly GameDbContext _context;
    private readonly GameResponseMapper _responseMapper;

    public GameStateService(GameDbContext context, GameResponseMapper responseMapper)
    {
        _context = context;
        _responseMapper = responseMapper;
    }

    public async Task<Game> LoadAsync(int gameId)
    {
        // Include laadt de gekoppelde speler direct mee.
        var game = await _context.Games
            .Include(item => item.Player)
            .FirstOrDefaultAsync(item => item.Id == gameId);

        if (game is null)
            throw new KeyNotFoundException($"Spel {gameId} bestaat niet.");

        return game;
    }

    public async Task SaveAsync(Game game)
    {
        game.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public GameResponse CreateResponse(Game game, string message = "")
    {
        return _responseMapper.Create(game, message);
    }
}
