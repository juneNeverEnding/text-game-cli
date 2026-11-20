namespace TextGameCLI.Core;

public interface IInteractiveCommand : ICommand
{
    bool IsDialogueActive { get; }

    CommandResult Receive(string input);
}