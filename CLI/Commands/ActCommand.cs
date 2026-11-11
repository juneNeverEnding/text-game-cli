namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Core;
using TextGameCLI.Core.Characters;
using TextGameCLI.Application;

public class ActCommand(GameManager manager) : ICommand
{
    public string Name => "act";
    public string Description => "act <attack|heal|defend|ability> <actor> [target] [--id <ability>]";
    private readonly GameManager _manager = manager;

    public CommandResult Execute(string[] args)
    {
        if (args.Length < 2)
            return CommandResult.Fail("usage: act <attack|heal|defend|ability> <actor> [target] [--id <ability>]");
        
        string action = args[0].ToLower();
        string actor = args[1];
        string? target = args.Length >= 3 ? args [2] : null;
        string? abilityId = null;

        for (int i = 2; i < args.Length; i++)
        {
            if((args[i] == "--id" || args[i] == "-i") && i + 1 < args.Length)
            {
                abilityId = args[++i];
                break;
            }                
        }

        switch (action)
        {
            case "attack":
                if (target is null) return CommandResult.Fail("attacking requires a target");

                _manager.Attack(actor, target);
                return CommandResult.Ok();
            
            case "heal":
                if (target is null) return CommandResult.Fail("healing requires a target");

                _manager.Heal(actor, target, 20);
                return CommandResult.Ok();
            
            case "defend":
                _manager.Defend(actor);
                return CommandResult.Ok();
            
            case "ability":
                if (target is null)
                    return CommandResult.Fail("ability requires a target");
                if (abilityId is null)
                    return CommandResult.Fail("you must specify the ability with --id <name>");
                
                _manager.UseAbility(actor, abilityId, target);
                return CommandResult.Ok();
            
            default:
                return CommandResult.Fail("unknown action. use: attack, heal, defend or ability");
        }
    }
}