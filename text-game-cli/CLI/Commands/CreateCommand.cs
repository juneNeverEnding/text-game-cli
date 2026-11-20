namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Core;
using TextGameCLI.Application;
using TextGameCLI.Core.Characters;

public class CreateCommand(GameManager manager) : IInteractiveCommand
{
    public string Name => "create";
    public string Description => "create <char|item|ability>";
    private readonly GameManager _manager = manager;

    private DialogueStep _step = DialogueStep.None;
    private string _name = "";
    private int _health, _armor, _power, _magic;

    public bool IsDialogueActive => _step != DialogueStep.None;

    public CommandResult Execute(string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("usage: create <char|item|ability>");
        
        return args[0].ToLower() switch
        {
            "char" or "character" => Step(DialogueStep.CharacterName, "name: "),
            "item" => Step(DialogueStep.ItemType, "available: armor, war_armor, robes, sword, staff, strength_pot, magic_pot, health_pot"),
            "ability" => Step(DialogueStep.AbilityType, "available: lightning_bolt, fireball, heal"),
            _ => CommandResult.Fail("unknown type. use char, item or ability")
        };
    }

    public CommandResult Receive(string input)
    {
        if (string.Equals(input.Trim(), "cancel", StringComparison.OrdinalIgnoreCase))
        {
            Reset();
            return CommandResult.Ok("creation cancelled");
        }

        return _step switch
        {
            DialogueStep.CharacterName => ReceiveName(input),
            DialogueStep.CharacterHealth => TryParseInt(input, out _health, DialogueStep.CharacterArmor, "armor: "),
            DialogueStep.CharacterArmor => TryParseInt(input, out _armor, DialogueStep.CharacterPower, "power: "),
            DialogueStep.CharacterPower => TryParseInt(input, out _power, DialogueStep.CharacterMagic, "magic: "),
            DialogueStep.CharacterMagic => TryParseInt(input, out _magic, DialogueStep.CharacterResistance, "resistance: "),
            DialogueStep.CharacterResistance => CreateCharacter(input),
            DialogueStep.ItemType => CreateItem(input),
            DialogueStep.AbilityType => CreateAbility(input),
            _ => CommandResult.Fail("no active dialogue")
        };
    }

    private CommandResult ReceiveName(string input)
    {
        _name = input.Trim();
        if (string.IsNullOrWhiteSpace(_name))
            _name = "unknown";
        
        return Step(DialogueStep.CharacterHealth, "health: ");
    }

    private CommandResult CreateCharacter(string input)
    {
        int.TryParse(input, out int intResistance);

        double resistance = intResistance / 100.0;
        var character = new Character(_name, _health > 0 ? _health : 100, _armor, _power, _magic, resistance);
        _manager.AddCharacter(character);

        Reset();
        return CommandResult.Ok($"character '{_name}' created");
    }

    private CommandResult CreateItem(string input)
    {
        IItem? item = input.Trim().ToLower() switch
        {
            "armor" => new Armor(),
            "war_armor" => new WarriorArmor(),
            "robes" => new Robes(),
            "sword" => new Sword(),
            "staff" => new Staff(),
            "strength_pot" => new StrengthPotion(),
            "magic_pot" => new MagicPotion(),
            "health_pot" => new HealthPotion(),
            "resistance_pot" => new ResistancePotion(),
            _ => null
        };

        if (item is null) return CommandResult.Fail("unknown item");

        _manager.RegisterItem(item);
        Reset();
        return CommandResult.Ok($"item '{item.Name}' created");
    }

    private CommandResult CreateAbility(string input)
    {
        IAbility? ability = input.Trim().ToLower() switch
        {
            "lightning_bolt" => new LightningBolt(),
            "fireball" => new Fireball(),
            "heal" => new Heal(),
            _ => null
        };

        if (ability is null) return CommandResult.Fail("unknown ability");

        _manager.RegisterAbility(ability);
        Reset();
        return CommandResult.Ok($"ability '{ability.Name}' created");
    }

    private CommandResult TryParseInt(string input, out int targetField, DialogueStep nextStep, string nextPrompt)
    {
        int.TryParse(input, out targetField);
        return Step(nextStep, nextPrompt);
    }

    private CommandResult Step(DialogueStep nextStep, string prompt)
    {
        _step = nextStep;
        return CommandResult.Ok(prompt);
    }

    private void Reset()
    {
        _step = DialogueStep.None;
        _name = "";
        _health = _armor = _power = _magic = 0;
    }
    private enum DialogueStep { None, CharacterName, CharacterHealth, CharacterArmor, CharacterPower, CharacterMagic, CharacterResistance, ItemType, AbilityType }
}