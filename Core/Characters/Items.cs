namespace TextGameCLI.Core.Characters;

/// <summary>
/// items
/// </summary>

public class Armor : IItem
{
    public string Name => "Armor";
    public void Apply(Character character)
    {
        character.ModifyStats(armorDelta: 10);
        Console.WriteLine($"{character.Name}'s armor stat update: {character.Armor}");
    }
}

public class WarriorArmor : IItem
{
    public string Name => "Warrior Armor";
    public void Apply(Character character)
    {
        character.ModifyStats(armorDelta: 6);
        character.ModifyStats(powerDelta: 6);
        Console.WriteLine($"{character.Name}'s stat update: armor: {character.Armor}, power: {character.Power}");
    }
}

public class Robes : IItem
{
    public string Name => "Mage Robes";
    public void Apply(Character character)
    {
        character.ModifyStats(armorDelta: 2);
        character.ModifyStats(magicDelta: 6);
        Console.WriteLine($"{character.Name}'s stat update: armor: {character.Armor}, magic: {character.Magic}");
    }
}

public class Sword : IItem
{
    public string Name => "Sword";
    public void Apply(Character character)
    {
        character.ModifyStats(powerDelta: 10);
        Console.WriteLine($"{character.Name}'s stat update: power: {character.Power}");
    }
}

public class Staff : IItem
{
    public string Name => "Staff";
    public void Apply(Character character)
    {
        character.ModifyStats(powerDelta: 2);
        character.ModifyStats(magicDelta: 7);
        Console.WriteLine($"{character.Name}'s stat update: power: {character.Power}, magic: {character.Magic}");
    }
}

public class StrengthPotion : IItem
{
    public string Name => "Strength Potion";
    public void Apply(Character character)
    {
        character.ModifyStats(powerDelta: 5);
        Console.WriteLine($"{character.Name}'s stat update: power: {character.Power}");
    }
}

public class MagicPotion : IItem
{
    public string Name => "Magic Potion";
    public void Apply(Character character)
    {
        character.ModifyStats(magicDelta: 5);
        Console.WriteLine($"{character.Name}'s stat update: magic: {character.Magic}");
    }
}

public class HealthPotion : IItem
{
    public string Name => "Health Potion";
    public void Apply(Character character)
    {
        character.ReceiveHeal(25);
    }
}