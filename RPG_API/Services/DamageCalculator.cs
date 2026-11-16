namespace RPG_API.Services;

// Hulpmethodes voor levens. Dit is de verbeterde versie van de
// ApplyDamage-oefening van de pagina "Debuggen in C#": levens worden nooit
// negatief en springen ook niet terug naar 100.
public static class DamageCalculator
{
    public static int ApplyDamage(int health, int damage)
    {
        if (damage < 0)
            damage = 0;

        return Math.Max(0, health - damage);
    }

    public static int Heal(int health, int amount, int maxHealth)
    {
        if (amount < 0)
            amount = 0;

        return Math.Min(maxHealth, health + amount);
    }
}
