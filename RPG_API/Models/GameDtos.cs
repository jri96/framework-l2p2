namespace RPG_API.Models;

// DTO's (Data Transfer Objects) zijn de JSON-berichten tussen console en API.

public class StatusResponse
{
    public string Message { get; set; } = "";
    public DateTime ServerTime { get; set; }
}

// Week 2: spelinformatie die de speler vooraf ziet.
public class GameInfoResponse
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int MaxLevel { get; set; }
}
