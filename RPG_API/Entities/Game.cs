using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RPG_API.Enums;

namespace RPG_API.Entities;

// De volledige toestand (state) van één spel. Alles wat nodig is om een spel
// te hervatten staat in deze tabel en in de gekoppelde Player.
public class Game
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Player))]
    public int PlayerId { get; set; }
    public virtual Player Player { get; set; } = null!;

    [MaxLength(100)]
    public string CurrentLocation { get; set; } = "";
    public int CurrentLevel { get; set; } = 1;
    public RoomType CurrentRoomType { get; set; } = RoomType.Start;

    // Relatie naar de vijand waartegen nu gevochten wordt (null = geen gevecht).
    [ForeignKey(nameof(CurrentEnemy))]
    public int? CurrentEnemyId { get; set; }
    public virtual Enemy? CurrentEnemy { get; set; }

    // De vijand-tabel bevat de vaste stats. De resterende levens van de vijand
    // in DIT spel worden hier bewaard.
    public int? CurrentEnemyHealth { get; set; }

    public int EnemiesDefeated { get; set; }
    public bool IsFinished { get; set; }
    public bool IsWon { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
