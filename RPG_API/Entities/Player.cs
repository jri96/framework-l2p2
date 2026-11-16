using System.ComponentModel.DataAnnotations;
using RPG_API.Services;

namespace RPG_API.Entities;

// Encapsulatie (week 6): levens, aanval en drankjes kun je van buitenaf alleen
// lezen. Veranderen gaat via de methodes hieronder, zodat de regels
// (nooit onder 0, nooit boven het maximum) op één plek staan.
// EF Core kan private setters gewoon vullen bij het laden uit de database.
public class Player
{
    [Key]
    public int Id { get; set; }

    [MaxLength(GameRules.MaxPlayerNameLength)]
    public required string Name { get; set; }

    public int Health { get; private set; } = GameRules.PlayerMaxHealth;
    public int MaxHealth { get; private set; } = GameRules.PlayerMaxHealth;
    public int Attack { get; private set; } = GameRules.PlayerStartAttack;

    // Eigen toevoeging: aantal genezingsdrankjes dat de speler bij zich heeft.
    public int Potions { get; private set; } = GameRules.StartPotions;

    public bool IsDefeated => Health == 0;
    public bool HasFullHealth => Health >= MaxHealth;

    public void TakeDamage(int damage)
    {
        Health = DamageCalculator.ApplyDamage(Health, damage);
    }

    // Geeft terug hoeveel HP er echt bij kwam.
    public int Heal(int amount)
    {
        var oldHealth = Health;
        Health = DamageCalculator.Heal(Health, amount, MaxHealth);
        return Health - oldHealth;
    }

    public void RestoreFullHealth()
    {
        Health = MaxHealth;
    }

    public void IncreaseAttack(int amount)
    {
        Attack += Math.Max(0, amount);
    }

    public bool TryAddPotion()
    {
        if (Potions >= GameRules.MaxPotions)
            return false;

        Potions++;
        return true;
    }

    public void UsePotion()
    {
        if (Potions <= 0)
            throw new InvalidOperationException("Er is geen drankje om te gebruiken.");

        Potions--;
    }
}
