using RPG_Console.Api;
using RPG_Console.Models;

namespace RPG_Console;

// De besturing van de console: menu's tonen, invoer lezen en de API aanroepen.
// De spelregels zelf staan in de API; de console laat alleen het resultaat zien.
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

        ConsoleRenderer.ShowWelcome(game.Player.Name);
        await PlayAsync(game);
    }

    private async Task ShowGameInfoAsync()
    {
        var info = await _gameApi.GetGameInfoAsync();
        ConsoleRenderer.ShowInfo(info);
        ConsoleInput.Pause();
    }

    // Lopende spellen kun je verder spelen; afgelopen spellen kun je alleen bekijken.
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

    private async Task PlayAsync(GameState game)
    {
        while (!game.IsFinished)
        {
            ConsoleRenderer.RenderGame(game);

            var inCombat = game.Enemy is not null;
            var option = ConsoleInput.ReadOption(inCombat ? 6 : 4);

            try
            {
                var nextGame = inCombat
                    ? await HandleCombatChoiceAsync(option, game)
                    : await HandleExplorationChoiceAsync(option, game);

                // null betekent: terug naar het hoofdmenu
                if (nextGame is null)
                    return;

                game = nextGame;
            }
            catch (ApiException exception)
            {
                // Bijvoorbeeld 409: "Je hebt geen genezingsdrankjes meer."
                ConsoleRenderer.ShowError(exception.Message);
                ConsoleInput.Pause();
            }
        }

        ConsoleRenderer.RenderGame(game);
        ConsoleRenderer.ShowEnding(game);
        ConsoleInput.Pause();
    }

    private async Task<GameState?> HandleExplorationChoiceAsync(int option, GameState game)
    {
        switch (option)
        {
            case 1:
                return await _gameApi.MoveAsync(game.Id);

            case 2:
                return await _gameApi.UsePotionAsync(game.Id);

            case 3:
                await ShowStatusAsync(game);
                return game;

            default:
                return null;
        }
    }

    private async Task<GameState?> HandleCombatChoiceAsync(int option, GameState game)
    {
        switch (option)
        {
            case 1:
                return await _gameApi.AttackAsync(game.Id, "Light");

            case 2:
                return await _gameApi.AttackAsync(game.Id, "Heavy");

            case 3:
                return await _gameApi.UsePotionAsync(game.Id);

            case 4:
                return await _gameApi.RunAsync(game.Id);

            case 5:
                await ShowStatusAsync(game);
                return game;

            default:
                return null;
        }
    }

    // Haalt de opgeslagen toestand opnieuw op via GET /api/game/{id}.
    private async Task ShowStatusAsync(GameState game)
    {
        var savedGame = await _gameApi.GetGameAsync(game.Id);
        ConsoleRenderer.ShowStatus(savedGame);
        ConsoleInput.Pause();
    }
}
