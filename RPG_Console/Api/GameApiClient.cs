using System.Net.Http.Json;
using System.Text.Json;
using RPG_Console.Models;

namespace RPG_Console.Api;

// Alle HTTP-aanroepen naar de API staan in deze class.
// De rest van de console weet niet hoe de URL's eruitzien.
public class GameApiClient
{
    private readonly HttpClient _httpClient;

    public GameApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> IsOnlineAsync()
    {
        try
        {
            var status = await _httpClient.GetFromJsonAsync<ApiStatus>("api/status");
            return status is not null;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return false;
        }
    }

    public async Task<GameInfo> GetGameInfoAsync()
    {
        var response = await _httpClient.GetAsync("api/info");
        return await ReadAsync<GameInfo>(response);
    }

    public async Task<GameState> StartGameAsync(string playerName)
    {
        var response = await _httpClient.PostAsJsonAsync("api/game", new { PlayerName = playerName });
        return await ReadAsync<GameState>(response);
    }

    public async Task<List<GameSummary>> GetGamesAsync()
    {
        var response = await _httpClient.GetAsync("api/game");
        return await ReadAsync<List<GameSummary>>(response);
    }

    public async Task<GameState> GetGameAsync(int gameId)
    {
        var response = await _httpClient.GetAsync($"api/game/{gameId}");
        return await ReadAsync<GameState>(response);
    }

    public async Task<GameState> MoveAsync(int gameId)
    {
        var response = await _httpClient.PostAsync($"api/game/{gameId}/move", null);
        return await ReadAsync<GameState>(response);
    }

    public async Task<GameState> AttackAsync(int gameId, string attackType)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/game/{gameId}/attack", new { Type = attackType });
        return await ReadAsync<GameState>(response);
    }

    public async Task<GameState> RunAsync(int gameId)
    {
        var response = await _httpClient.PostAsync($"api/game/{gameId}/run", null);
        return await ReadAsync<GameState>(response);
    }

    public async Task<GameState> UsePotionAsync(int gameId)
    {
        var response = await _httpClient.PostAsync($"api/game/{gameId}/potion", null);
        return await ReadAsync<GameState>(response);
    }

    // Controleert de statuscode. Bij een fout wordt de JSON-foutmelding van de
    // API gelezen en als ApiException gegooid.
    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            throw await CreateExceptionAsync(response);

        var result = await response.Content.ReadFromJsonAsync<T>();
        return result ?? throw new ApiException((int)response.StatusCode, "De API stuurde een leeg antwoord.");
    }

    private static async Task<ApiException> CreateExceptionAsync(HttpResponseMessage response)
    {
        var statusCode = (int)response.StatusCode;
        var body = await response.Content.ReadAsStringAsync();

        try
        {
            var error = JsonSerializer.Deserialize<ApiError>(body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            if (!string.IsNullOrWhiteSpace(error?.Error))
                return new ApiException(statusCode, error.Error);
        }
        catch (JsonException)
        {
            // Geen JSON: val terug op de standaardmelding hieronder.
        }

        return new ApiException(statusCode, $"De API gaf een fout terug ({statusCode}).");
    }
}
