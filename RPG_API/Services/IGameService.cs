using RPG_API.Models;

namespace RPG_API.Services;

// De controller kent alleen deze interface, niet de class die het werk doet.
// Dependency Injection (Program.cs) koppelt IGameService aan GameService.
public interface IGameService
{
    Task<GameResponse> StartAsync(StartGameRequest request);
    Task<List<GameSummaryResponse>> GetGamesAsync();
    Task<GameResponse> GetAsync(int gameId);
    Task<GameResponse> MoveAsync(int gameId);
    Task<GameResponse> UsePotionAsync(int gameId);
}
