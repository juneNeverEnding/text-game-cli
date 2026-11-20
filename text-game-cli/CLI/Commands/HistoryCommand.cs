namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Core;
using TextGameCLI.Application;

public class HistoryCommand(GameManager manager) : ICommand
{
    public string Name => "history";
    public string Description => "shows current battle history";
    private readonly GameManager _manager = manager;

    public CommandResult Execute(string[] args)
    {
        _manager.ShowHistory();
        return CommandResult.Ok();
    }
}