namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Core;
using TextGameCLI.Application;

public class SelectCommand(GameManager manager) : ICommand
{
    public string Name => "select";
    public string Description => "select <char1> <char2> ...";

    private readonly GameManager _manager = manager;

    public CommandResult Execute(string[] args)
    {
        if (args.Length < 2)
            return CommandResult.Fail("select at least 2 characters");
        
        _manager.SelectCharacters(args);
        return CommandResult.Ok();
    }
}