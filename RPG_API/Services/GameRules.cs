namespace RPG_API.Services;

// Alle vaste spelgegevens op één plek.
public static class GameRules
{
    // Spelinformatie
    public const string Title = "Crypte van de Domtoren";
    public const string Description =
        "Klim door tien levels van een oude crypte naar de klokkenzolder. " +
        "Versla vijanden, rust uit bij kaarsen en versla de Klokkenwachter om de uitgang te bereiken.";

    // Levels
    public const int MaxLevel = 10;

    // Speler
    public const int MaxPlayerNameLength = 30;
    public const int PlayerMaxHealth = 100;

    private static readonly string[] LocationNames =
    {
        "",
        "Ingang van de crypte",
        "Vochtige gang",
        "Botkamer",
        "Vergeten kapel",
        "Kaarsenkamer",
        "Wenteltrap",
        "Overstroomde kelder",
        "Uurwerkzaal",
        "Torenkamer",
        "Klokkenzolder"
    };

    public static string GetLocationName(int level)
    {
        if (level < 1 || level > MaxLevel)
            return $"Level {level}";

        return LocationNames[level];
    }
}
