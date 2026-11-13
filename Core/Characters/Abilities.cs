namespace TextGameCLI.Core.Characters;

using TextGameCLI.Core.Combat;
///<summary>
/// abilities
///</summary>

public class LightningBolt : IAbility
{
    public string Name => "Lightning bolt";
    public int Execute(Character user, Character? target)
    {
        if (target is null) return 0;

        int magicDamage = DamageCalculator.CalculateMagic(user, target);
        target.TakeDamage(magicDamage);
        Console.WriteLine($"{user.Name} casts a {Name} on {target.Name} and dealt {magicDamage} damage\n{target.Name}'s current health: {target.Health}");
        return magicDamage;
    }
}

public class Fireball : IAbility
{
    public string Name => "Fireball";
    public int Execute(Character user, Character? target)
    {
        if (target is null) return 0;

        int magicDamage = DamageCalculator.CalculateMagic(user, target);
        target.TakeDamage(magicDamage);
        Console.WriteLine($"{user.Name} casts a {Name} on {target.Name} and dealt {magicDamage} damage\n{target.Name}'s current health: {target.Health}");
        return magicDamage;
    }
}

public class Heal : IAbility
{
    public string Name => "Heal";

    public int Execute(Character user, Character? target)
    {
        if (target is null) return 0;
        int HealPoints = (int)Math.Round(user.Magic * 0.9);
        user.Heal(HealPoints, target);
        return HealPoints;
    }
}