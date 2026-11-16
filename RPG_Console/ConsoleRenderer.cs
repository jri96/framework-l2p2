using RPG_Console.Models;

namespace RPG_Console;

// Alles wat met tekenen op het scherm te maken heeft.
public static class ConsoleRenderer
{
    private const int PanelWidth = 50;

    public static void ShowMainMenu(bool apiOnline)
    {
        ClearScreen();
        WriteTitle();

        if (apiOnline)
            WriteColored("  API: online", ConsoleColor.Green);
        else
            WriteColored("  API: offline - start eerst RPG_API", ConsoleColor.Red);

        Console.WriteLine();
        Console.WriteLine("  1. Nieuw spel");
        Console.WriteLine("  2. Spel hervatten");
        Console.WriteLine("  3. Spelinformatie");
        Console.WriteLine("  4. Afsluiten");
        Console.WriteLine();
    }

    public static void ShowInfo(GameInfo info)
    {
        ClearScreen();
        WriteTitle();
        WriteColored($"  {info.Title}", ConsoleColor.Yellow);
        Console.WriteLine();
        foreach (var line in WrapText(info.Description, PanelWidth - 2))
            Console.WriteLine($"  {line}");
        Console.WriteLine();
        Console.WriteLine($"  Aantal levels:   {info.MaxLevel}");
    }

    public static void ShowGames(List<GameSummary> games)
    {
        ClearScreen();
        WriteTitle();
        Console.WriteLine("  OPGESLAGEN SPELLEN");
        Console.WriteLine();

        for (var index = 0; index < games.Count; index++)
        {
            var game = games[index];
            Console.WriteLine($"  {index + 1}. {game.PlayerName} - level {game.CurrentLevel}: {game.CurrentLocation}");
            Console.WriteLine($"     HP {game.Health}/{game.MaxHealth} - laatst gespeeld {game.UpdatedAt.ToLocalTime():dd-MM HH:mm}");
        }

        Console.WriteLine($"  {games.Count + 1}. Terug");
        Console.WriteLine();
    }

    public static void RenderGame(GameState game)
    {
        ClearScreen();
        WriteTitle();
        DrawPlayerPanel(game);
        DrawMessage(game);

        Console.WriteLine();
        Console.WriteLine("  1. Status bekijken");
        Console.WriteLine("  2. Terug naar hoofdmenu");
        Console.WriteLine();
    }

    public static void ShowStatus(GameState game)
    {
        Console.WriteLine();
        WriteLine('-');
        Console.WriteLine($"  STATUS - spel #{game.Id}");
        WriteLine('-');
        Console.WriteLine($"  Speler:   {game.Player.Name}");
        Console.WriteLine($"  Levens:   {game.Player.Health}/{game.Player.MaxHealth}");
        Console.WriteLine($"  Level:    {game.CurrentLevel}/{game.MaxLevel} ({game.CurrentLocation})");
        Console.WriteLine("  Deze gegevens komen opnieuw uit de database (GET /api/game/{id}).");
    }

    public static void ShowGoodbye()
    {
        Console.WriteLine();
        Console.WriteLine("  Tot de volgende keer, avonturier.");
    }

    public static void ShowError(string message)
    {
        Console.WriteLine();
        WriteColored($"  ! {message}", ConsoleColor.Red);
    }

    public static void ShowWarning(string message)
    {
        WriteColored($"  {message}", ConsoleColor.Yellow);
    }

    private static void DrawPlayerPanel(GameState game)
    {
        var player = game.Player;
        WriteLine('=');
        Console.WriteLine($"  {player.Name}   |   spel #{game.Id}");
        Console.Write("  HP    ");
        WriteBar(player.Health, player.MaxHealth);
        Console.WriteLine($" {player.Health}/{player.MaxHealth}");
        Console.Write("  Level ");
        WriteBar(game.CurrentLevel, game.MaxLevel, ConsoleColor.Cyan);
        Console.WriteLine($" {game.CurrentLevel}/{game.MaxLevel}");
        Console.WriteLine($"  Locatie: {game.CurrentLocation}");
        WriteLine('=');
    }

    private static void DrawMessage(GameState game)
    {
        if (string.IsNullOrWhiteSpace(game.Message))
            return;

        Console.WriteLine();
        foreach (var line in WrapText(game.Message, PanelWidth - 2))
            Console.WriteLine($"  {line}");
    }

    private static void WriteTitle()
    {
        WriteLine('=');
        WriteColored("     C R Y P T E   V A N   D E   D O M T O R E N", ConsoleColor.Yellow);
        WriteColored("          een dungeon crawler in tien levels", ConsoleColor.DarkGray);
        WriteLine('=');
        Console.WriteLine();
    }

    private static void WriteBar(int value, int maximum, ConsoleColor? color = null)
    {
        const int barSize = 20;
        var filled = maximum <= 0 ? 0 : Math.Clamp(value * barSize / maximum, 0, barSize);

        var percentage = value * 100 / Math.Max(1, maximum);
        var barColor = color ?? percentage switch
        {
            > 60 => ConsoleColor.Green,
            > 30 => ConsoleColor.Yellow,
            _ => ConsoleColor.Red
        };

        Console.Write("[");
        Console.ForegroundColor = barColor;
        Console.Write(new string('#', filled));
        Console.ResetColor();
        Console.Write(new string('-', barSize - filled));
        Console.Write("]");
    }

    private static void WriteLine(char character)
    {
        Console.WriteLine(new string(character, PanelWidth));
    }

    private static void WriteColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    private static IEnumerable<string> WrapText(string text, int width)
    {
        var line = "";
        foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.Length + word.Length + 1 > width && line.Length > 0)
            {
                yield return line;
                line = "";
            }

            line = line.Length == 0 ? word : $"{line} {word}";
        }

        if (line.Length > 0)
            yield return line;
    }

    private static void ClearScreen()
    {
        if (!Console.IsOutputRedirected)
            Console.Clear();
        else
            Console.WriteLine();
    }
}
