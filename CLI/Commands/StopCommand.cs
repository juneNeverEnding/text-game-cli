namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Core;
using TextGameCLI.Application;

public class StopCommand(GameManager manager) : ICommand
{
    public string Name => "stop";
    public string Description => "stop - ends the current battle";

    private readonly GameManager _manager = manager;

    public CommandResult Execute(string[] args)
    {
        _manager.StopBattle();
        return CommandResult.Ok();
    }
}