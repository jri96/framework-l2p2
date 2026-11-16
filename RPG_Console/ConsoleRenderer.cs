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
        Console.WriteLine("  2. Spel hervatten of bekijken");
        Console.WriteLine("  3. Spelinformatie");
        Console.WriteLine("  4. Afsluiten");
        Console.WriteLine();
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
            var status = game.IsFinished ? (game.IsWon ? "gewonnen" : "game over") : "bezig";
            Console.WriteLine($"  {index + 1}. {game.PlayerName} - level {game.CurrentLevel}: {game.CurrentLocation} [{status}]");
            Console.WriteLine($"     HP {game.Health}/{game.MaxHealth} - laatst gespeeld {game.UpdatedAt.ToLocalTime():dd-MM HH:mm}");
        }

        Console.WriteLine($"  {games.Count + 1}. Terug");
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
        Console.WriteLine($"  Rustplaatsen:    level {string.Join(" en ", info.RestLevels)}");
        Console.WriteLine($"  Eindbaas:        level {info.BossLevel}");
        Console.WriteLine($"  Start-HP:        {info.StartHealth}");
        Console.WriteLine($"  Drankjes:        {info.StartPotions}");
    }

    public static void RenderGame(GameState game)
    {
        ClearScreen();
        WriteTitle();
        DrawPlayerPanel(game);
        DrawMessage(game);

        if (game.IsFinished)
            return;

        if (game.Enemy is not null)
            DrawCombatPanel(game.Enemy);
        else
            DrawExplorationPanel(game);
    }

    public static void ShowStatus(GameState game)
    {
        Console.WriteLine();
        WriteLine('-');
        Console.WriteLine($"  STATUS - spel #{game.Id}");
        WriteLine('-');
        Console.WriteLine($"  Speler:          {game.Player.Name}");
        Console.WriteLine($"  Levens:          {game.Player.Health}/{game.Player.MaxHealth}");
        Console.WriteLine($"  Aanvalskracht:   {game.Player.Attack}");
        Console.WriteLine($"  Drankjes:        {game.Player.Potions}");
        Console.WriteLine($"  Level:           {game.CurrentLevel}/{game.MaxLevel} ({game.CurrentLocation})");
        Console.WriteLine($"  Ruimte:          {TranslateRoom(game.CurrentRoomType)}");
        Console.WriteLine($"  Vijanden verslagen: {game.EnemiesDefeated}");
        Console.WriteLine("  Het spel wordt na iedere actie automatisch opgeslagen.");
    }

    public static void ShowEnding(GameState game)
    {
        Console.WriteLine();
        if (game.IsWon)
        {
            WriteColored("  *** GEWONNEN! ***", ConsoleColor.Yellow);
            Console.WriteLine($"  {game.Player.Name} versloeg {game.EnemiesDefeated} vijanden en de Klokkenwachter.");
        }
        else
        {
            WriteColored("  *** GAME OVER ***", ConsoleColor.Red);
            Console.WriteLine($"  {game.Player.Name} kwam tot level {game.CurrentLevel} en versloeg {game.EnemiesDefeated} vijanden.");
        }
    }

    public static void ShowWelcome(string playerName)
    {
        WriteColored($"  Succes, {playerName}!", ConsoleColor.Cyan);
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
        Console.WriteLine($"  {player.Name}   |   Aanval {player.Attack}   |   Drankjes {player.Potions}");
        Console.Write("  HP    ");
        WriteBar(player.Health, player.MaxHealth);
        Console.WriteLine($" {player.Health}/{player.MaxHealth}");
        Console.Write("  Level ");
        WriteBar(game.CurrentLevel, game.MaxLevel, ConsoleColor.Cyan);
        Console.WriteLine($" {game.CurrentLevel}/{game.MaxLevel}");
        Console.WriteLine($"  Locatie: {game.CurrentLocation} ({TranslateRoom(game.CurrentRoomType)})");
        WriteLine('=');
    }

    private static void DrawMessage(GameState game)
    {
        if (string.IsNullOrWhiteSpace(game.Message))
            return;

        Console.WriteLine();
        foreach (var line in WrapText(game.Message, PanelWidth - 2))
            Console.WriteLine($"  {line}");

        if (game.PlayerAttack is not null || game.EnemyAttack is not null)
        {
            Console.WriteLine();
            WriteColored($"  Jij deed {game.DamageDealt} schade  |  Jij kreeg {game.DamageReceived} schade", ConsoleColor.DarkGray);
        }
    }

    private static void DrawCombatPanel(EnemyState enemy)
    {
        Console.WriteLine();
        WriteColored(enemy.IsBoss ? "  [ EINDBAAS ]" : "  [ GEVECHT ]", ConsoleColor.Red);
        foreach (var line in EnemyArt.Get(enemy.Name))
            Console.WriteLine($"      {line}");

        Console.WriteLine($"  {enemy.Name} - {enemy.Description}");
        Console.Write("  HP    ");
        WriteBar(enemy.Health, enemy.MaxHealth, ConsoleColor.Red);
        Console.WriteLine($" {enemy.Health}/{enemy.MaxHealth}");
        Console.WriteLine($"  Schade {enemy.Damage}  |  Snelheid {enemy.Speed}");
        Console.WriteLine();
        Console.WriteLine("  1. Lichte aanval (snel, minder schade)");
        Console.WriteLine("  2. Zware aanval (traag, meer schade)");
        Console.WriteLine("  3. Drankje drinken (kost je beurt)");
        Console.WriteLine(enemy.IsBoss ? "  4. Vluchten (kan niet bij de eindbaas)" : "  4. Vluchten (60% kans)");
        Console.WriteLine("  5. Status bekijken");
        Console.WriteLine("  6. Terug naar hoofdmenu");
        Console.WriteLine();
    }

    private static void DrawExplorationPanel(GameState game)
    {
        Console.WriteLine();
        WriteColored("  [ VERKENNEN ]", ConsoleColor.Green);
        var nextText = game.CurrentLevel + 1 == game.MaxLevel ? "Naar de klokkenzolder (eindbaas!)" : "Verder naar het volgende level";
        Console.WriteLine($"  1. {nextText}");
        Console.WriteLine("  2. Drankje drinken");
        Console.WriteLine("  3. Status bekijken");
        Console.WriteLine("  4. Terug naar hoofdmenu");
        Console.WriteLine();
    }

    private static string TranslateRoom(string roomType)
    {
        return roomType switch
        {
            "Start" => "startpunt",
            "Normal" => "lege ruimte",
            "Combat" => "gevecht",
            "Treasure" => "schatkamer",
            "Rest" => "rustplaats",
            "Boss" => "eindbaas",
            "Exit" => "uitgang",
            _ => roomType
        };
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
        // Console.Clear() werkt niet als de uitvoer wordt doorgestuurd (bijv. naar een bestand).
        if (!Console.IsOutputRedirected)
            Console.Clear();
        else
            Console.WriteLine();
    }
}
