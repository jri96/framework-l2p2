using RPG_API.Models;

namespace RPG_API.Services;

// Eén ingang voor de controller. Stuurt iedere actie door naar de service
// die er verantwoordelijk voor is.
public class GameService : IGameService
{
    private readonly GameStartService _gameStart;
    private readonly GameQueryService _gameQuery;

    public GameService(GameStartService gameStart, GameQueryService gameQuery)
    {
        _gameStart = gameStart;
        _gameQuery = gameQuery;
    }

    public Task<GameResponse> StartAsync(StartGameRequest request) => _gameStart.StartAsync(request);
    public Task<List<GameSummaryResponse>> GetGamesAsync() => _gameQuery.GetGamesAsync();
    public Task<GameResponse> GetAsync(int gameId) => _gameQuery.GetAsync(gameId);
}
