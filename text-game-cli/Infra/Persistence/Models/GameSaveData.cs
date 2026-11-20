namespace TextGameCLI.Infra.Persistence.Models;

public record GameSaveData
{
    public string SaveName { get; set; } = "";
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    public List<CharacterData> Characters { get; set; } = [];
    public List<AbilityData> Abilities { get; set; } = [];
    public List<ItemData> Items { get; set; } = [];
    public List<string> SelectedCharacterNames { get; set; } = [];
    public bool IsBattleActive { get; set; }
    public List<TurnData> BattleHistory { get; set; } = [];
}