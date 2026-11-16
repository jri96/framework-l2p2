using RPG_Console.Api;
using RPG_Console.Models;

namespace RPG_Console;

// De besturing van de console: menu's tonen, invoer lezen en de API aanroepen.
public class ConsoleGame
{
    private readonly GameApiClient _gameApi;

    public ConsoleGame(GameApiClient gameApi)
    {
        _gameApi = gameApi;
    }

    public async Task RunAsync()
    {
        try
        {
            await RunMainMenuAsync();
        }
        catch (InputClosedException)
        {
            // Invoer is gesloten: netjes stoppen.
        }

        ConsoleRenderer.ShowGoodbye();
    }

    private async Task RunMainMenuAsync()
    {
        var running = true;

        while (running)
        {
            var apiOnline = await _gameApi.IsOnlineAsync();
            ConsoleRenderer.ShowMainMenu(apiOnline);
            var option = ConsoleInput.ReadOption(4);

            try
            {
                switch (option)
                {
                    case 1:
                        await StartNewGameAsync();
                        break;

                    case 2:
                        await ContinueGameAsync();
                        break;

                    case 3:
                        await ShowGameInfoAsync();
                        break;

                    case 4:
                        running = false;
                        break;
                }
            }
            catch (ApiException exception)
            {
                ConsoleRenderer.ShowError(exception.Message);
                ConsoleInput.Pause();
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                ConsoleRenderer.ShowError("De API is niet bereikbaar. Start eerst de API met: dotnet run --project RPG_API");
                ConsoleInput.Pause();
            }
        }
    }

    private async Task StartNewGameAsync()
    {
        var playerName = ConsoleInput.ReadRequired("  Naam van je held: ");
        var game = await _gameApi.StartGameAsync(playerName);
        await PlayAsync(game);
    }

    private async Task ShowGameInfoAsync()
    {
        var info = await _gameApi.GetGameInfoAsync();
        ConsoleRenderer.ShowInfo(info);
        ConsoleInput.Pause();
    }

    private async Task ContinueGameAsync()
    {
        var games = await _gameApi.GetGamesAsync();

        if (games.Count == 0)
        {
            ConsoleRenderer.ShowWarning("Er zijn nog geen opgeslagen spellen. Start een nieuw spel.");
            ConsoleInput.Pause();
            return;
        }

        ConsoleRenderer.ShowGames(games);
        var option = ConsoleInput.ReadOption(games.Count + 1);

        if (option == games.Count + 1)
            return;

        var game = await _gameApi.GetGameAsync(games[option - 1].Id);

        // Een afgelopen spel mag je bekijken, maar niet verder spelen.
        if (game.IsFinished)
        {
            ConsoleRenderer.RenderGame(game);
            ConsoleRenderer.ShowEnding(game);
            ConsoleInput.Pause();
            return;
        }

        game.Message = "Je pakt je avontuur weer op.";
        await PlayAsync(game);
    }

    // De console stuurt alleen de keuze. Level, ruimte en gezondheid bepaalt de API.
    private async Task PlayAsync(GameState game)
    {
        while (!game.IsFinished)
        {
            ConsoleRenderer.RenderGame(game);
            var option = ConsoleInput.ReadOption(4);

            try
            {
                switch (option)
                {
                    case 1:
                        game = await _gameApi.MoveAsync(game.Id);
                        break;

                    case 2:
                        game = await _gameApi.UsePotionAsync(game.Id);
                        break;

                    case 3:
                        var savedGame = await _gameApi.GetGameAsync(game.Id);
                        ConsoleRenderer.ShowStatus(savedGame);
                        ConsoleInput.Pause();
                        break;

                    default:
                        return;
                }
            }
            catch (ApiException exception)
            {
                ConsoleRenderer.ShowError(exception.Message);
                ConsoleInput.Pause();
            }
        }

        ConsoleRenderer.RenderGame(game);
        ConsoleRenderer.ShowEnding(game);
        ConsoleInput.Pause();
    }
}
