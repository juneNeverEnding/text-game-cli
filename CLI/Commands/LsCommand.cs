namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Application;
using TextGameCLI.Core;

public class LsCommand(GameManager manager) : ICommand
{
    public string Name => "ls";
    public string Description => "ls <chars|items|abilities> [--id <name>]";
    private readonly GameManager _manager = manager;

    public CommandResult Execute(string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("usage: ls <chars|items|abilities> [--id <name>]");
        
        switch (args[0].ToLower())
        {
            case "chars" or "characters":
                _manager.ListCharacters();
                break;
            case "items":
                _manager.ListItems();
                break;
            case "abilities":
                _manager.ListAbilities();
                break;
            default:
                return CommandResult.Fail("unknown category");
        }

        return CommandResult.Ok();
    }
}