namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Application;
using TextGameCLI.Core;

public class SaveCommand(GameManager manager) : ICommand
{
    public string Name => "save";
    public string Description => "save <name>";
    private readonly GameManager _manager = manager;

    public CommandResult Execute(string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("usage: save <name>");
        
        string name = string.Join(" ", args);
        _manager.Save(name);
        return CommandResult.Ok();
    }
}