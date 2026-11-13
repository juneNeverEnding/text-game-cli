namespace TextGameCLI.Core.Characters;

using TextGameCLI.Core.Combat;
///<summary>
/// abilities
///</summary>

public class LightningBolt : IAbility
{
    public string Name => "Lightning bolt";
    public void Execute(Character user, Character? target)
    {
        if (target is null) return;

        int magicDamage = DamageCalculator.CalculateMagic(user, target);
        target.TakeDamage(magicDamage);
        Console.WriteLine($"{user.Name} casts a {Name} on {target.Name} and dealt {magicDamage} damage\n{target.Name}'s current health: {target.Health}");
    }
}

public class Fireball : IAbility
{
    public string Name => "Fireball";
    public void Execute(Character user, Character? target)
    {
        if (target is null) return;

        int magicDamage = DamageCalculator.CalculateMagic(user, target);
        target.TakeDamage(magicDamage);
        Console.WriteLine($"{user.Name} casts a {Name} on {target.Name} and dealt {magicDamage} damage\n{target.Name}'s current health: {target.Health}");
    }
}

public class Heal : IAbility
{
    public string Name => "Heal";

    public void Execute(Character user, Character? target)
    {
        if (target is null) return;
        int HealPoints = (int)Math.Round(user.Magic * 0.9);
        user.Heal(HealPoints, target);
    }
}