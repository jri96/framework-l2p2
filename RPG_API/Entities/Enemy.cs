using System.ComponentModel.DataAnnotations;

namespace RPG_API.Entities;

// Abstracte basisclass: er bestaat geen "gewone" Enemy, alleen subclasses.
// EF Core slaat alle subclasses op in één tabel (Table Per Hierarchy) met een
// extra kolom "Discriminator" die aangeeft welke class het is.
public abstract class Enemy
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = "";

    [MaxLength(200)]
    public string Description { get; set; } = "";

    public int MaxHealth { get; set; }
    public int Damage { get; set; }
    public int Speed { get; set; }
    public bool IsBoss { get; set; }
}
