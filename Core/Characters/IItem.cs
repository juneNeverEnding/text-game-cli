namespace TextGameCLI.Core.Characters;

/// <summary>
/// item template for the character
/// </summary>

public interface IItem
{
    string Name { get; }
    void Apply(Character character);
}