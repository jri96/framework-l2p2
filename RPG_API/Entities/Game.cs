using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RPG_API.Enums;

namespace RPG_API.Entities;

// De toestand (state) van één spel. Een game hoort bij precies één speler.
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

    public bool IsFinished { get; set; }
    public bool IsWon { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
