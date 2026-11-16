namespace RPG_Console;

public static class ConsoleInput
{
    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = ReadLine().Trim();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            ConsoleRenderer.ShowWarning("Je moet iets invullen.");
        }
    }

    public static int ReadOption(int maximum)
    {
        while (true)
        {
            var input = ReadRequired("Keuze > ");

            if (int.TryParse(input, out var option) && option >= 1 && option <= maximum)
                return option;

            ConsoleRenderer.ShowWarning($"Onbekende keuze. Kies een nummer van 1 tot en met {maximum}.");
        }
    }

    public static void Pause()
    {
        Console.WriteLine();
        Console.Write("Druk op Enter om verder te gaan...");
        ReadLine();
    }

    // Console.ReadLine() geeft null als de invoer gesloten is (bijv. Ctrl+Z / Ctrl+D).
    private static string ReadLine()
    {
        return Console.ReadLine() ?? throw new InputClosedException();
    }
}

public class InputClosedException : Exception
{
}
