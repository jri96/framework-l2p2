using RPG_API.Enums;

namespace RPG_API.Services;

// Rekent één aanval uit: hoeveel schade en hoe snel.
public class CombatService
{
    private readonly IRandomGenerator _random;

    public CombatService(IRandomGenerator random)
    {
        _random = random;
    }

    public AttackResult PerformAttack(int baseDamage, int baseSpeed, AttackType attackType)
    {
        int damage;
        var speed = baseSpeed;

        switch (attackType)
        {
            case AttackType.Light:
                // Licht: minder schade, maar sneller.
                damage = _random.Next(baseDamage - 2, baseDamage + 3);
                speed += GameRules.LightAttackSpeedBonus;
                break;

            case AttackType.Heavy:
                // Zwaar: meer schade, maar langzamer.
                damage = _random.Next(baseDamage + 3, baseDamage + 8);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(attackType));
        }

        return new AttackResult(attackType, Math.Max(1, damage), speed);
    }

    public AttackType ChooseEnemyAttack()
    {
        return _random.Chance(50) ? AttackType.Light : AttackType.Heavy;
    }
}

public class AttackResult
{
    public AttackResult(AttackType type, int damage, int speed)
    {
        Type = type;
        Damage = damage;
        Speed = speed;
    }

    public AttackType Type { get; }
    public int Damage { get; }
    public int Speed { get; }
}
