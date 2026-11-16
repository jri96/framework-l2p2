using RPG_API.Models;

namespace RPG_API.Services;

// Week 2/3: spelinformatie. De gegevens blijven in de API; de console toont ze alleen.
public interface IGameInfoService
{
    GameInfoResponse GetInfo();
}

public class GameInfoService : IGameInfoService
{
    public GameInfoResponse GetInfo()
    {
        return new GameInfoResponse
        {
            Title = GameRules.Title,
            Description = GameRules.Description,
            MaxLevel = GameRules.MaxLevel,
            RestLevels = GameRules.RestLevels.ToList(),
            BossLevel = GameRules.BossLevel,
            StartHealth = GameRules.PlayerMaxHealth,
            StartPotions = GameRules.StartPotions
        };
    }
}
