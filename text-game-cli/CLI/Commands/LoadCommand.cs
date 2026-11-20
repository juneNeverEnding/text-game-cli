namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Application;
using TextGameCLI.Core;

public class LoadCommand(GameManager manager) : ICommand
{
    public string Name => "load";
    public string Description => "load [name] - loads save, if there isnt a name it loads the last save";
    private readonly GameManager _manager = manager;

    public CommandResult Execute(string[] args)
    {
        string? name = args.Length > 0 ? string.Join(" ", args) : null;
        _manager.Load(name);
        return CommandResult.Ok();
    }
}