namespace TextGameCLI.Core;

public interface ICommandStrategy
{
    void Register(Action<string, ICommand> register);
}