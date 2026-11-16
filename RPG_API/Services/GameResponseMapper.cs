using RPG_API.Entities;
using RPG_API.Models;

namespace RPG_API.Services;

// Zet een Game-entity om naar de GameResponse-DTO die als JSON terug gaat.
public class GameResponseMapper
{
    public GameResponse Create(Game game, string message = "")
    {
        return new GameResponse
        {
            Id = game.Id,
            CurrentLevel = game.CurrentLevel,
            MaxLevel = GameRules.MaxLevel,
            CurrentLocation = game.CurrentLocation,
            IsFinished = game.IsFinished,
            IsWon = game.IsWon,
            Message = message.Trim(),
            Player = new PlayerResponse
            {
                Name = game.Player.Name,
                Health = game.Player.Health,
                MaxHealth = game.Player.MaxHealth
            }
        };
    }
}
