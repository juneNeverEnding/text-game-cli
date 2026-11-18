namespace TextGameCLI.Infra.Persistence.Models;

public record GameSaveData
{
    public string SaveName { get; init; } = "";
    public DateTime SavedAt { get; init; } = DateTime.UtcNow;
    public List<CharacterData> Characters { get; init; } = [];
    public List<AbilityData> Abilities { get; init; } = [];
    public List<ItemData> Items { get; init; } = [];
    public List<string> SelectedCharacterNames { get; init; } = [];
    public bool IsBattleActive { get; init; }
    public List<TurnData> BattleHistory { get; init; } = [];
}