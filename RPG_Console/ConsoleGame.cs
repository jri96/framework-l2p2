namespace RPG_Console;

// Week 1: menu met een lus. Nog geen verbinding met de API.
public class ConsoleGame
{
    public void Run()
    {
        try
        {
            var running = true;

            while (running)
            {
                ConsoleRenderer.ShowMainMenu();
                var option = ConsoleInput.ReadOption(2);

                switch (option)
                {
                    case 1:
                        StartNewGame();
                        break;

                    case 2:
                        running = false;
                        break;
                }
            }
        }
        catch (InputClosedException)
        {
            // Invoer is gesloten: netjes stoppen.
        }

        ConsoleRenderer.ShowGoodbye();
    }

    private static void StartNewGame()
    {
        var playerName = ConsoleInput.ReadRequired("  Naam van je held: ");
        ConsoleRenderer.ShowWelcome(playerName);
        ConsoleInput.Pause();
    }
}
