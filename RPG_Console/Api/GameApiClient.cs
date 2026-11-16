using System.Net.Http.Json;
using RPG_Console.Models;

namespace RPG_Console.Api;

// Alle HTTP-aanroepen naar de API staan in deze class.
// De console is de client, de API is de server.
public class GameApiClient
{
    private readonly HttpClient _httpClient;

    public GameApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET /api/status
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

    // GET /api/info
    public async Task<GameInfo> GetGameInfoAsync()
    {
        var info = await _httpClient.GetFromJsonAsync<GameInfo>("api/info");
        return info ?? throw new HttpRequestException("De API stuurde een leeg antwoord.");
    }
}
