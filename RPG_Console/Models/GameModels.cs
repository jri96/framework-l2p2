namespace RPG_Console.Models;

// Kopieën van de DTO's van de API. De namen komen overeen met de JSON-velden.

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
