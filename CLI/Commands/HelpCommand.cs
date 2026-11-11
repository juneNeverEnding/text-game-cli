namespace TextGameCLI.CLI.Commands;

using TextGameCLI.Core;

public class HelpCommand(Dictionary<string, ICommand> commands) : ICommand
{
    public string Name => "help";
    public string Description => "available commands";
    private readonly Dictionary<string, ICommand> _commands = commands;

    public CommandResult Execute(string[] args)
    {
        var lines = new List<string>
        {
            "available commands:",
            "-------------------"
        };

        foreach (var cmd in _commands.Values.OrderBy(c => c.Name))
            lines.Add($"{cmd.Name,-12} - {cmd.Description}");


        lines.AddRange(new[]
        {
            "", 
            "examples:",
            "   create char / item / ability",
            "   add --char Bob --item Sword",
            "   add -c Joe -i staff",
            "   add -c Joe -a \"Lightning bolt\"",
            "   ls chars",
            "   ls items",
            "   ls abilities",
            "   select Bob Joe Evilguy \"Another Joe\"",
            "   start",
            "   act attack Bob Evilguy",
            "   act ability Joe Evilguy --id \"Lightning bolt\"",
            "   act ability Evilguy Joe -i Fireball",
            "   act ability \"Another Joe\" Bob --id Heal",
            "   help",
            "   exit"
        });

        return CommandResult.Ok(string.Join("\n", lines));
    }
}