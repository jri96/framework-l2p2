namespace RPG_Console.Models;

// Kopieën van de DTO's van de API. De console kent de database-entities niet.

public class GameState
{
    public int Id { get; set; }
    public int CurrentLevel { get; set; }
    public int MaxLevel { get; set; }
    public string CurrentLocation { get; set; } = "";
    public string CurrentRoomType { get; set; } = "";
    public bool IsFinished { get; set; }
    public bool IsWon { get; set; }
    public string Message { get; set; } = "";
    public PlayerState Player { get; set; } = new();
}

public class PlayerState
{
    public string Name { get; set; } = "";
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Potions { get; set; }
}

public class GameSummary
{
    public int Id { get; set; }
    public string PlayerName { get; set; } = "";
    public int CurrentLevel { get; set; }
    public string CurrentLocation { get; set; } = "";
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public bool IsFinished { get; set; }
    public bool IsWon { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ApiStatus
{
    public string Message { get; set; } = "";
    public DateTime ServerTime { get; set; }
}

public class GameInfo
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int MaxLevel { get; set; }
    public List<int> RestLevels { get; set; } = new();
    public int StartHealth { get; set; }
    public int StartPotions { get; set; }
}
