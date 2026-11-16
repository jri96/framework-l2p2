using System.Text;
using RPG_Console;
using RPG_Console.Api;

Console.OutputEncoding = Encoding.UTF8;

// Het adres van de API. Moet gelijk zijn aan applicationUrl in RPG_API/Properties/launchSettings.json.
var apiUrl = Environment.GetEnvironmentVariable("DUNGEON_API_URL") ?? "http://localhost:5265/";
if (!apiUrl.EndsWith('/'))
    apiUrl += "/";

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(apiUrl),
    Timeout = TimeSpan.FromSeconds(10)
};

var game = new ConsoleGame(new GameApiClient(httpClient));
await game.RunAsync();
