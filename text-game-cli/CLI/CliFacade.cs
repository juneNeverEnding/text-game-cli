namespace TextGameCLI.CLI;

using TextGameCLI.Core;

public class CliFacade(IRenderer renderer)
{
    private readonly ArgParser _parser = new();
    private readonly Dictionary<string, Func<string[], CommandResult>> _commands = new(StringComparer.OrdinalIgnoreCase);
    private readonly IRenderer _renderer = renderer;

    public void UseStrategy(ICommandStrategy strategy)
    {
        _commands.Clear();
        strategy.Register((name, handler) => _commands[name] = handler);
    }

    public CommandResult Execute(string input)
    {
        var (command, args) = _parser.Parse(input);

        if (string.IsNullOrEmpty(command))
            return CommandResult.Ok();
        
        if (_commands.TryGetValue(command, out var handler))
            return handler(args);

        return CommandResult.Fail($"unknown command '{command}'. type 'help'");
    }

    public void Display(string message) => _renderer.WriteLine(message);
}