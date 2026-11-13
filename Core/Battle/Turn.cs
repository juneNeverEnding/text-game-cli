namespace TextGameCLI.Core.Battle;

public class Turn
{
    public int Number { get; set; }
    public string Actor { get; set; } = "";
    public string Action { get; set; } = "";
    public string? Target { get; set; }
    public int Value { get; set; }
    public string Description { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public override string ToString()
        => $"#{Number:D3} | {Actor, -12} | {(Target ?? "-"), -12} | {Value, 4} | {Description}";
}