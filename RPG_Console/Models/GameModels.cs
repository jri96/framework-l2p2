namespace RPG_Console.Models;

// Kopieën van de DTO's van de API. De console kent de database-entities niet.

public class GameState
{
    public int Id { get; set; }
    public int CurrentLevel { get; set; }
    public int MaxLevel { get; set; }
    public string CurrentLocation { get; set; } = "";
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
}
