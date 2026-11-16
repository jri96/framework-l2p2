namespace RPG_API.Entities;

public class Orc : Enemy
{
    public Orc()
    {
        Name = "Ork";
        Description = "Traag, maar een klap van deze knots doet pijn.";
        MaxHealth = 65;
        Damage = 12;
        Speed = 6;
    }
}
