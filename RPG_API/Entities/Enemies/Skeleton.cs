namespace RPG_API.Entities;

public class Skeleton : Enemy
{
    public Skeleton()
    {
        Name = "Skelet";
        Description = "Een rammelend skelet met een roestig zwaard.";
        MaxHealth = 40;
        Damage = 8;
        Speed = 10;
    }
}
