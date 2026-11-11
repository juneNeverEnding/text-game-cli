namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Core;
using TextGameCLI.Application;
using TextGameCLI.Core.Characters;

public class CreateCommand(GameManager manager) : ICommand
{
    public string Name => "create";
    public string Description => "create <char|item|ability>";
    private readonly GameManager _manager = manager;

    public CommandResult Execute(string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("usage: create <char|item|ability>");
        
        return args[0].ToLower() switch
        {
            "char" or "character" => CreateCharacter(),
            "item" => CreateItem(),
            "ability" => CreateAbility(),
            _ => CommandResult.Fail("unknown type. use char, item or ability")
        };
    }

    private CommandResult CreateCharacter()
    {
        Console.Write("name: ");
        string name = Console.ReadLine() ?? "unknown";
        Console.Write("health: ");
        int.TryParse(Console.ReadLine(), out int health);
        Console.Write("armor: ");
        int.TryParse(Console.ReadLine(), out int armor);
        Console.Write("power: ");
        int.TryParse(Console.ReadLine(), out int power);
        Console.Write("magic: ");
        int.TryParse(Console.ReadLine(), out int magic);

        var character = new Character(name, health > 0 ? health : 100, armor, power, magic);
        _manager.AddCharacter(character);

        return CommandResult.Ok($"character '{name}' created");
    }

    private CommandResult CreateItem()
    {
        Console.WriteLine("available: armor, war_armor, robes, sword, staff, strength_pot, magic_pot, health_pot");
        Console.Write("item: ");
        string type = (Console.ReadLine() ?? "").ToLower();

        IItem? item = type switch
        {
            "armor" => new Armor(),
            "war_armor" => new WarriorArmor(),
            "robes" => new Robes(),
            "sword" => new Sword(),
            "staff" => new Staff(),
            "strength_pot" => new StrengthPotion(),
            "magic_pot" => new MagicPotion(),
            "heatlh_pot" => new HealthPotion(),
            _ => null
        };

        if (item is null) return CommandResult.Fail("unknown item");

        _manager.RegisterItem(item);
        return CommandResult.Ok($"item '{item.Name}' created");
    }

    private CommandResult CreateAbility()
    {
        Console.WriteLine("available: lightning_bolt, fireball, heal");
        Console.Write("ability: ");
        string type = (Console.ReadLine() ?? "").ToLower();

        IAbility? ability = type switch
        {
            "lightning_bolt" => new LightningBolt(),
            "fireball" => new Fireball(),
            "heal" => new Heal(),
            _ => null
        };

        if (ability is null) return CommandResult.Fail("unknown ability");

        _manager.RegisterAbility(ability);
        return CommandResult.Ok($"ability '{ability.Name}' created");
    }
}