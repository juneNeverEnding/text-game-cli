namespace TextGameCLI.Core;

public class CommandResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
    public static CommandResult Ok(string msg = "") => new() { Success = true, Message = msg };
    public static CommandResult Fail(string msg) => new() { Success = false, Message = msg };
}