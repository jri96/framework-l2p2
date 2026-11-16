using System.ComponentModel.DataAnnotations;
using RPG_API.Enums;
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

public class AttackRequest
{
    [EnumDataType(typeof(AttackType), ErrorMessage = "Kies een geldige aanval: Light of Heavy.")]
    public AttackType Type { get; set; }
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

public class GameResponse
{
    public int Id { get; set; }
    public int CurrentLevel { get; set; }
    public int MaxLevel { get; set; }
    public string CurrentLocation { get; set; } = "";
    public string CurrentRoomType { get; set; } = "";
    public int EnemiesDefeated { get; set; }
    public bool IsFinished { get; set; }
    public bool IsWon { get; set; }
    public string Message { get; set; } = "";
    public int DamageDealt { get; set; }
    public int DamageReceived { get; set; }
    public string? PlayerAttack { get; set; }
    public string? EnemyAttack { get; set; }
    public PlayerResponse Player { get; set; } = new();
    public EnemyResponse? Enemy { get; set; }
}

public class PlayerResponse
{
    public string Name { get; set; } = "";
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Attack { get; set; }
    public int Potions { get; set; }
}

public class EnemyResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Damage { get; set; }
    public int Speed { get; set; }
    public bool IsBoss { get; set; }
}

public class StatusResponse
{
    public string Message { get; set; } = "";
    public DateTime ServerTime { get; set; }
}

// Week 2/3: spelinformatie die de speler vooraf ziet.
public class GameInfoResponse
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int MaxLevel { get; set; }
    public List<int> RestLevels { get; set; } = new();
    public int BossLevel { get; set; }
    public int StartHealth { get; set; }
    public int StartPotions { get; set; }
}
