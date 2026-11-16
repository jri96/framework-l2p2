using RPG_API.Enums;
using RPG_API.Models;

namespace RPG_API.Services;

// Eén ingang voor de controller. Stuurt iedere actie door naar de service
// die er verantwoordelijk voor is.
public class GameService : IGameService
{
    private readonly GameStartService _gameStart;
    private readonly GameQueryService _gameQuery;
    private readonly MovementService _movement;
    private readonly CombatGameService _combat;
    private readonly PotionService _potion;

    public GameService(
        GameStartService gameStart,
        GameQueryService gameQuery,
        MovementService movement,
        CombatGameService combat,
        PotionService potion)
    {
        _gameStart = gameStart;
        _gameQuery = gameQuery;
        _movement = movement;
        _combat = combat;
        _potion = potion;
    }

    public Task<GameResponse> StartAsync(StartGameRequest request) => _gameStart.StartAsync(request);
    public Task<List<GameSummaryResponse>> GetGamesAsync() => _gameQuery.GetGamesAsync();
    public Task<GameResponse> GetAsync(int gameId) => _gameQuery.GetAsync(gameId);
    public Task<GameResponse> MoveAsync(int gameId) => _movement.MoveAsync(gameId);
    public Task<GameResponse> AttackAsync(int gameId, AttackType attackType) => _combat.AttackAsync(gameId, attackType);
    public Task<GameResponse> RunAsync(int gameId) => _combat.RunAsync(gameId);
    public Task<GameResponse> UsePotionAsync(int gameId) => _potion.UsePotionAsync(gameId);
}
