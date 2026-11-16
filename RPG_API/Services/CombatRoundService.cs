using RPG_API.Entities;
using RPG_API.Enums;

namespace RPG_API.Services;

// Voert één gevechtsronde uit: speler en vijand vallen aan, de snelste eerst.
// Deze class past alleen de game state aan; opslaan gebeurt in de aanroepende service.
public class CombatRoundService
{
    private readonly CombatService _combat;
    private readonly GameStateService _gameState;
    private readonly IRandomGenerator _random;

    public CombatRoundService(CombatService combat, GameStateService gameState, IRandomGenerator random)
    {
        _combat = combat;
        _gameState = gameState;
        _random = random;
    }

    public CombatRoundResult Resolve(Game game, AttackType playerAttackType)
    {
        var enemy = game.CurrentEnemy!;
        var playerAttack = _combat.PerformAttack(game.Player.Attack, GameRules.PlayerSpeed, playerAttackType);
        var enemyAttack = _combat.PerformAttack(enemy.Damage, enemy.Speed, _combat.ChooseEnemyAttack());

        var result = new CombatRoundResult
        {
            PlayerAttack = playerAttack.Type.ToString(),
            EnemyAttack = enemyAttack.Type.ToString()
        };

        // Bij gelijke snelheid mag de speler eerst.
        if (playerAttack.Speed >= enemyAttack.Speed)
        {
            ApplyPlayerAttack(game, enemy, playerAttack, result);

            // Tegenaanval alleen als de vijand nog leeft.
            if (game.CurrentEnemy is not null)
                ApplyEnemyAttack(game, enemy, enemyAttack, result);
        }
        else
        {
            result.Message += $"De {enemy.Name} is sneller en valt als eerste aan. ";
            ApplyEnemyAttack(game, enemy, enemyAttack, result);

            if (!game.IsFinished)
                ApplyPlayerAttack(game, enemy, playerAttack, result);
        }

        return result;
    }

    // Gebruikt als de speler zijn beurt aan iets anders besteedt
    // (een drankje drinken of een mislukte vluchtpoging).
    public CombatRoundResult EnemyFreeAttack(Game game)
    {
        var enemy = game.CurrentEnemy!;
        var enemyAttack = _combat.PerformAttack(enemy.Damage, enemy.Speed, _combat.ChooseEnemyAttack());
        var result = new CombatRoundResult { EnemyAttack = enemyAttack.Type.ToString() };

        ApplyEnemyAttack(game, enemy, enemyAttack, result);
        return result;
    }

    private void ApplyPlayerAttack(Game game, Enemy enemy, AttackResult attack, CombatRoundResult result)
    {
        result.DamageDealt = attack.Damage;
        var enemyHealth = game.CurrentEnemyHealth ?? enemy.MaxHealth;
        game.CurrentEnemyHealth = DamageCalculator.ApplyDamage(enemyHealth, attack.Damage);

        var attackName = attack.Type == AttackType.Light ? "lichte" : "zware";
        result.Message += $"Je doet een {attackName} aanval en raakt de {enemy.Name} voor {attack.Damage} schade. ";

        if (game.CurrentEnemyHealth == 0)
            HandleVictory(game, enemy, result);
    }

    private static void ApplyEnemyAttack(Game game, Enemy enemy, AttackResult attack, CombatRoundResult result)
    {
        result.DamageReceived = attack.Damage;
        game.Player.TakeDamage(attack.Damage);

        var attackName = attack.Type == AttackType.Light ? "snelle" : "harde";
        result.Message += $"De {enemy.Name} doet een {attackName} aanval: {attack.Damage} schade. ";

        if (game.Player.IsDefeated)
        {
            game.IsFinished = true;
            game.IsWon = false;
            result.Message += "Je zakt in elkaar... Game over. ";
        }
    }

    // Eigen spelregel: na iedere overwinning word je sterker en heb je kans op een drankje.
    private void HandleVictory(Game game, Enemy enemy, CombatRoundResult result)
    {
        _gameState.ClearEnemy(game);
        game.EnemiesDefeated++;
        game.Player.IncreaseAttack(GameRules.AttackBonusPerVictory);
        result.Message += $"De {enemy.Name} is verslagen! Je aanvalskracht stijgt naar {game.Player.Attack}. ";

        // Eindbaas verslagen: de uitgang is bereikt en het spel is gewonnen.
        if (enemy.IsBoss)
        {
            game.CurrentRoomType = RoomType.Exit;
            game.IsFinished = true;
            game.IsWon = true;
            result.Message += "De klokken vallen stil en achter de Klokkenwachter gaat de uitgang open. Je hebt gewonnen! ";
            return;
        }

        if (_random.Chance(GameRules.PotionDropChance) && game.Player.TryAddPotion())
            result.Message += "Hij laat een genezingsdrankje vallen. ";
    }
}

public class CombatRoundResult
{
    public string Message { get; set; } = "";
    public int DamageDealt { get; set; }
    public int DamageReceived { get; set; }
    public string? PlayerAttack { get; set; }
    public string? EnemyAttack { get; set; }
}
