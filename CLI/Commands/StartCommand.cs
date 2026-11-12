namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Core;
using TextGameCLI.Application;

public class StartCommand(GameManager manager) : ICommand
{
    public string Name => "start";
    public string Description => "starts the battle";

    private readonly GameManager _manager = manager;
    
    public CommandResult Execute(string[] args)
    {
        _manager.StartBattle();
        return CommandResult.Ok();
    }
}