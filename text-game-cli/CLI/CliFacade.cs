namespace TextGameCLI.CLI;

using TextGameCLI.Core;

public class CliFacade(IRenderer renderer)
{
    private readonly ArgParser _parser = new();
    private readonly Dictionary<string, ICommand> _commands = new(StringComparer.OrdinalIgnoreCase);
    private IInteractiveCommand? _activeDialogue;
    private readonly IRenderer _renderer = renderer;

    public void UseStrategy(ICommandStrategy strategy)
    {
        _commands.Clear();
    }

    public CommandResult Execute(string input)
    {
        if (_activeDialogue != null)
        {
            var result = _activeDialogue.Receive(input);

            if (!_activeDialogue.IsDialogueActive)
                _activeDialogue = null;

            return result;
        }
        var (commandName, args) = _parser.Parse(input);

        if (string.IsNullOrEmpty(commandName))
            return CommandResult.Ok();
        
        if (!_commands.TryGetValue(commandName, out var command))
            return CommandResult.Fail($"unknown command '{commandName}'. type 'help'");

        var commandResult = command.Execute(args);

        if (command is IInteractiveCommand interactive && interactive.IsDialogueActive)
            _activeDialogue = interactive;
        
        return commandResult;
    }

    public void Display(string message) => _renderer.WriteLine(message);
}