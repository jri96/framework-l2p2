using RPG_API.Entities;
using RPG_API.Models;

namespace RPG_API.Services;

// Zet een Game-entity om naar de GameResponse-DTO die als JSON terug gaat.
public class GameResponseMapper
{
    public GameResponse Create(Game game, string message = "", CombatRoundResult? round = null)
    {
        var response = new GameResponse
        {
            Id = game.Id,
            CurrentLevel = game.CurrentLevel,
            MaxLevel = GameRules.MaxLevel,
            CurrentLocation = game.CurrentLocation,
            CurrentRoomType = game.CurrentRoomType.ToString(),
            EnemiesDefeated = game.EnemiesDefeated,
            IsFinished = game.IsFinished,
            IsWon = game.IsWon,
            Message = message.Trim(),
            DamageDealt = round?.DamageDealt ?? 0,
            DamageReceived = round?.DamageReceived ?? 0,
            PlayerAttack = round?.PlayerAttack,
            EnemyAttack = round?.EnemyAttack,
            Player = new PlayerResponse
            {
                Name = game.Player.Name,
                Health = game.Player.Health,
                MaxHealth = game.Player.MaxHealth,
                Attack = game.Player.Attack,
                Potions = game.Player.Potions
            }
        };

        if (game.CurrentEnemy is not null)
        {
            response.Enemy = new EnemyResponse
            {
                Id = game.CurrentEnemy.Id,
                Name = game.CurrentEnemy.Name,
                Description = game.CurrentEnemy.Description,
                Health = game.CurrentEnemyHealth ?? game.CurrentEnemy.MaxHealth,
                MaxHealth = game.CurrentEnemy.MaxHealth,
                Damage = game.CurrentEnemy.Damage,
                Speed = game.CurrentEnemy.Speed,
                IsBoss = game.CurrentEnemy.IsBoss
            };
        }

        return response;
    }
}
