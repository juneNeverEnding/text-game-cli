namespace TextGameCLI.Core.Memento;

using TextGameCLI.Infra.Persistence.Models;

public class GameMemento(string name, List<CharacterData> characters, List<AbilityData> abilities, List<ItemData> items, List<string> selectedCharacterNames, bool isBattleActive, List<TurnData> battleHistory) : IMemento
{
    public string Name { get;} = name;

    internal List<CharacterData> Characters { get; } = characters;
    internal List<AbilityData> Abilities { get; } = abilities;
    internal List<ItemData> Items { get; } = items;
    internal List<string> SelectedCharacterNames { get; } = selectedCharacterNames;
    internal bool IsBattleActive { get; } = isBattleActive;
    internal List<TurnData> BattleHistory { get; } = battleHistory;
}