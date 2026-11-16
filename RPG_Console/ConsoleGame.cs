using RPG_Console.Api;

namespace RPG_Console;

// Week 2: de console haalt gegevens op bij de API via HTTP en JSON.
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
            var option = ConsoleInput.ReadOption(3);

            try
            {
                switch (option)
                {
                    case 1:
                        StartNewGame();
                        break;

                    case 2:
                        await ShowGameInfoAsync();
                        break;

                    case 3:
                        running = false;
                        break;
                }
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                ConsoleRenderer.ShowError("De API is niet bereikbaar. Start eerst de API met: dotnet run --project RPG_API");
                ConsoleInput.Pause();
            }
        }
    }

    private static void StartNewGame()
    {
        var playerName = ConsoleInput.ReadRequired("  Naam van je held: ");
        ConsoleRenderer.ShowWelcome(playerName);
        ConsoleInput.Pause();
    }

    private async Task ShowGameInfoAsync()
    {
        var info = await _gameApi.GetGameInfoAsync();
        ConsoleRenderer.ShowInfo(info);
        ConsoleInput.Pause();
    }
}
