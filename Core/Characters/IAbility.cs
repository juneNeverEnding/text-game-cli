namespace TextGameCLI.Core.Characters;

/// <summary>
/// ability template for the character
/// </summary>

public interface IAbility
{
    string Name { get; }
    void Execute(Character user, Character? target);
}