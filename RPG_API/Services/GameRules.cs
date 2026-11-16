using RPG_API.Enums;

namespace RPG_API.Services;

// Alle getallen van de spelregels op één plek. Zo kun je de balans aanpassen
// zonder door alle services te zoeken.
public static class GameRules
{
    // Spelinformatie
    public const string Title = "Crypte van de Domtoren";
    public const string Description =
        "Klim door tien levels van een oude crypte naar de klokkenzolder. " +
        "Versla vijanden, rust uit bij kaarsen en versla de Klokkenwachter om de uitgang te bereiken.";

    // Levels
    public const int MaxLevel = 10;
    public const int BossLevel = 10;
    public static readonly int[] RestLevels = { 5, 9 };

    // Speler
    public const int MaxPlayerNameLength = 30;
    public const int PlayerMaxHealth = 100;
    public const int PlayerStartAttack = 10;
    public const int PlayerSpeed = 10;

    // Aanvallen
    public const int LightAttackSpeedBonus = 10;

    // Eigen regel: drankjes
    public const int StartPotions = 2;
    public const int MaxPotions = 5;
    public const int PotionHealAmount = 30;
    public const int PotionDropChance = 25;

    // Eigen regel: vluchten
    public const int RunSuccessChance = 60;

    // Eigen regel: sterker worden na een overwinning
    public const int AttackBonusPerVictory = 1;

    // Verdeling van de willekeurige ruimtes (de rest is een lege ruimte)
    public const int CombatRoomChance = 65;
    public const int TreasureRoomChance = 20;

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

    public static bool IsRestLevel(int level)
    {
        return RestLevels.Contains(level);
    }

    public static RoomType ChooseRandomRoom(int roll)
    {
        if (roll < CombatRoomChance)
            return RoomType.Combat;

        if (roll < CombatRoomChance + TreasureRoomChance)
            return RoomType.Treasure;

        return RoomType.Normal;
    }
}
