namespace RPG_API.Entities;

public class Goblin : Enemy
{
    public Goblin()
    {
        Name = "Kobold";
        Description = "Klein, snel en gemeen. Valt vaak als eerste aan.";
        MaxHealth = 30;
        Damage = 6;
        Speed = 16;
    }
}
