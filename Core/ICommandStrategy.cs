namespace TextGameCLI.Core;

public interface ICommandStrategy
{
    IEnumerable<string> GetCommandNames();
    void Register(Action<string, Func<string[], CommandResult>> register);
}