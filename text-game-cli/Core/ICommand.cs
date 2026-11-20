namespace TextGameCLI.Core;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    CommandResult Execute(string[] args);
}