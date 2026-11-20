namespace TextGameCLI.Infra.Persistence.Models;

public record TurnData(int Number, string Actor, string Action, string? Target, int Value, string Description);