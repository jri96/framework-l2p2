using Microsoft.EntityFrameworkCore;
using RPG_API.Database;
using RPG_API.Entities;
using RPG_API.Exceptions;
using RPG_API.Models;

namespace RPG_API.Services;

// Gedeelde stappen die iedere spelactie nodig heeft:
// laden -> controleren -> (actie) -> opslaan -> response maken.
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
        var game = await _context.Games
            .Include(item => item.Player)
            .FirstOrDefaultAsync(item => item.Id == gameId);

        if (game is null)
            throw new GameNotFoundException(gameId);

        return game;
    }

    // Een beëindigde game mag nog bekeken worden, maar krijgt geen nieuwe spelacties.
    public void EnsureActive(Game game)
    {
        if (game.IsFinished)
            throw new GameRuleException("Dit spel is al afgelopen. Start een nieuw spel.");
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
