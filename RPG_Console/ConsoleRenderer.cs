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
        Console.WriteLine("  2. Spelinformatie");
        Console.WriteLine("  3. Afsluiten");
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

    public static void ShowWelcome(string playerName)
    {
        Console.WriteLine();
        WriteColored($"  Welkom, {playerName}!", ConsoleColor.Cyan);
        Console.WriteLine("  Je staat bij de ingang van de crypte onder de Domtoren.");
        Console.WriteLine("  Hoog boven je luidt de Klokkenwachter...");
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

    private static void WriteTitle()
    {
        WriteLine('=');
        WriteColored("     C R Y P T E   V A N   D E   D O M T O R E N", ConsoleColor.Yellow);
        WriteColored("          een dungeon crawler in tien levels", ConsoleColor.DarkGray);
        WriteLine('=');
        Console.WriteLine();
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
