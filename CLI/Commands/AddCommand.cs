namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Core;
using TextGameCLI.Core.Characters;
using TextGameCLI.Application;

public class AddCommand(GameManager manager) : ICommand
{
    public string Name => "add";
    public string Description => "add [--char <name>] [--item|--ability <name>]";
    private readonly GameManager _manager = manager;

    public CommandResult Execute(string[] args)
    {
        string? charName = null;
        string? itemName = null;
        string? abilityName = null;

        for (int i = 0; i < args.Length; i++)
        {
            if ((args[i] == "--char" || args[i] == "-c") && i + 1 < args.Length)
                charName = args[++i];
            else if ((args[i] == "--item" || args[i] == "-i") && i + 1 < args.Length)
                itemName = args[++i];
            else if ((args[i] == "--ability" || args[i] == "-a") && i + 1 < args.Length)
                abilityName = args[++i];
        }

        if (charName == null)
            return CommandResult.Fail("you must specify a character: --char <name>");

        if (itemName != null)
        {   
            _manager.EquipItem(charName, itemName);
            return CommandResult.Ok($"item '{itemName}' added to {charName}");
        }

        if (abilityName != null)
        {            
            _manager.AddAbilityToCharacter(charName, abilityName);
            return CommandResult.Ok($"ability '{abilityName}' added to {charName}");
        }
        return CommandResult.Fail("specify either --item or --ability");
    }
}