using System.ComponentModel.DataAnnotations;
using RPG_API.Services;

namespace RPG_API.Entities;

public class Player
{
    [Key]
    public int Id { get; set; }

    [MaxLength(GameRules.MaxPlayerNameLength)]
    public required string Name { get; set; }

    public int Health { get; set; } = GameRules.PlayerMaxHealth;
    public int MaxHealth { get; set; } = GameRules.PlayerMaxHealth;

    // Eigen toevoeging: aantal genezingsdrankjes dat de speler bij zich heeft.
    public int Potions { get; set; } = GameRules.StartPotions;
}
