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
        };
        _commands["help"] = new HelpCommand(_commands);
    }

    public void Register(Action<string, Func<string[], CommandResult>> register)
    {
        foreach (var (name, command) in _commands)
        {
            register(name, command.Execute);
        }
    }
    public IEnumerable<string> GetCommandNames() => _commands.Keys;
}