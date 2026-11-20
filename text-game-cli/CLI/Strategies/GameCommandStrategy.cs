namespace TextGameCLI.CLI.Strategies;

using TextGameCLI.Core;
using TextGameCLI.CLI.Commands;
using TextGameCLI.Application;

public class GameCommandStrategy : ICommandStrategy
{
    private readonly Dictionary<string, ICommand> _commands;

    public GameCommandStrategy(GameManager manager)
    {
        _commands = new(StringComparer.OrdinalIgnoreCase)
        {
            ["create"]  = new CreateCommand(manager),
            ["add"]     = new AddCommand(manager),
            ["act"]     = new ActCommand(manager),
            ["ls"]      = new LsCommand(manager),
            ["select"]  = new SelectCommand(manager),
            ["start"]   = new StartCommand(manager),
            ["stop"]    = new StopCommand(manager),
            ["history"] = new HistoryCommand(manager),
            ["save"]    = new SaveCommand(manager),
            ["load"]    = new LoadCommand(manager)
        };
        _commands["help"] = new HelpCommand(_commands);
    }

    public void Register(Action<string, ICommand> register)
    {
        foreach (var (name, command) in _commands)
            register(name, command);
    }
}