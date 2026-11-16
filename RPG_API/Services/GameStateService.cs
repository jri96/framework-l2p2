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
            .Include(item => item.CurrentEnemy)
            .FirstOrDefaultAsync(item => item.Id == gameId);

        if (game is null)
            throw new GameNotFoundException(gameId);

        return game;
    }

    public void EnsureActive(Game game)
    {
        if (game.IsFinished)
            throw new GameRuleException("Dit spel is al afgelopen. Start een nieuw spel.");
    }

    public void EnsureInCombat(Game game)
    {
        if (game.CurrentEnemy is null)
            throw new GameRuleException("Er is hier geen vijand om tegen te vechten.");
    }

    public void EnsureNotInCombat(Game game)
    {
        if (game.CurrentEnemy is not null)
            throw new GameRuleException($"De {game.CurrentEnemy.Name} staat nog voor je. Versla hem of probeer te vluchten.");
    }

    public void ClearEnemy(Game game)
    {
        game.CurrentEnemy = null;
        game.CurrentEnemyId = null;
        game.CurrentEnemyHealth = null;
    }

    public async Task SaveAsync(Game game)
    {
        game.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public GameResponse CreateResponse(Game game, string message = "", CombatRoundResult? round = null)
    {
        return _responseMapper.Create(game, message, round);
    }
}
