namespace RPG_API.Entities;

// Eigen eindbaas op level 10. Voor een boss kun je niet vluchten (zie CombatGameService).
public class BellWarden : Enemy
{
    public BellWarden()
    {
        Name = "Klokkenwachter";
        Description = "De bewaker van de klokkenzolder. Elke slag galmt door de toren.";
        MaxHealth = 120;
        Damage = 12;
        Speed = 12;
        IsBoss = true;
    }
}
