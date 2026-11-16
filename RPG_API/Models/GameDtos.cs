using System.ComponentModel.DataAnnotations;
using RPG_API.Services;

namespace RPG_API.Models;

// DTO's (Data Transfer Objects) zijn de JSON-berichten tussen console en API.
// Zo stuurt de API nooit direct een database-entity naar buiten.

public class StartGameRequest
{
    [Required(ErrorMessage = "Vul een spelernaam in.")]
    [StringLength(GameRules.MaxPlayerNameLength, ErrorMessage = "De spelernaam mag maximaal 30 tekens lang zijn.")]
    public string PlayerName { get; set; } = "";
}

public class GameSummaryResponse
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

// Alles wat de console nodig heeft om een spel te tonen en te hervatten.
public class GameResponse
{
    public int Id { get; set; }
    public int CurrentLevel { get; set; }
    public int MaxLevel { get; set; }
    public string CurrentLocation { get; set; } = "";
    public bool IsFinished { get; set; }
    public bool IsWon { get; set; }
    public string Message { get; set; } = "";
    public PlayerResponse Player { get; set; } = new();
}

public class PlayerResponse
{
    public string Name { get; set; } = "";
    public int Health { get; set; }
    public int MaxHealth { get; set; }
}

public class StatusResponse
{
    public string Message { get; set; } = "";
    public DateTime ServerTime { get; set; }
}

public class GameInfoResponse
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int MaxLevel { get; set; }
}
