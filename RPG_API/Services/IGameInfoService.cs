using RPG_API.Models;

namespace RPG_API.Services;

// Week 3: spelinformatie via een service. De controller roept deze interface aan.
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
            StartHealth = GameRules.PlayerMaxHealth,
            StartPotions = GameRules.StartPotions
        };
    }
}
