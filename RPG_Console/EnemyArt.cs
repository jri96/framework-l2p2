namespace RPG_Console;

// ASCII-tekeningen per vijand. Puur presentatie, daarom in de console.
public static class EnemyArt
{
    public static string[] Get(string enemyName)
    {
        return enemyName switch
        {
            "Skelet" => new[]
            {
                "   .-.  ",
                "  (o.o) ",
                "   |=|  ",
                "  /|_|\\-+",
                "   / \\  "
            },
            "Kobold" => new[]
            {
                "  /\\_/\\ ",
                " ( >.< )",
                "  /| |\\ ",
                "   ^ ^  "
            },
            "Ork" => new[]
            {
                "   _____  ",
                "  ( >_< ) ",
                " /|=====|\\",
                "  |_| |_| "
            },
            "Klokkenwachter" => new[]
            {
                "      _|_      ",
                "     /   \\     ",
                "    | o o |    ",
                "   /|  ^  |\\   ",
                "  /_|_____|_\\  ",
                "     DONG!     "
            },
            _ => new[] { "  ( ? )  " }
        };
    }
}
