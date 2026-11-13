namespace TextGameCLI.Core.Battle;

using TextGameCLI.Core.Characters;

public class Battle
{
    public string Id { get; } = Guid.NewGuid().ToString("N")[..8];
    public List<Character> Participants { get; } = [];
    public List<Turn> History { get; } = [];
    public bool IsActive { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    private int _turnNumber = 1;

    public void Start(IEnumerable<Character> characters)
    {
        Participants.Clear();
        Participants.AddRange(characters);
        History.Clear();
        _turnNumber = 1;
        IsActive = true;
        StartedAt = DateTime.Now;
        EndedAt = null;
    }

    public void Stop()
    {
        IsActive = false;
        EndedAt = DateTime.Now;
    }

    public void Record(string actor, string action, string? target, int value, string description)
    {
        History.Add(new Turn
        {
            Number = _turnNumber++,
            Actor = actor,
            Action = action,
            Target = target,
            Value = value,
            Description = description
        });
    }
}