namespace TextGameCLI.Infra.Persistence.Models;

public record CharacterData(string Name, int MaxHealth, int Health, int Armor, int Power, int Magic, double Resistance, bool IsAlive, List<string> AbilityNames, List<string> ItemNames);