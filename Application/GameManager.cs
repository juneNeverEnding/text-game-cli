namespace TextGameCLI.Application;

using TextGameCLI.Core;
using TextGameCLI.Core.Characters;

public class GameManager(IRenderer renderer)
{
    private readonly IRenderer _renderer = renderer;
    private readonly List<Character> _characters = [];
    private readonly List<IAbility> _abilities = [];
    private readonly List<IItem> _items = [];
    private readonly List<Character> _selectedForBattle = [];
    private bool _isBattleStarted = false;

    public IReadOnlyList<Character> Characters => _characters.AsReadOnly();
    public IReadOnlyList<Character> Selected => _selectedForBattle.AsReadOnly();
    public bool IsBattleStarted => _isBattleStarted;

    public void AddCharacter(Character character)
    {
        if (_isBattleStarted)
        {
            _renderer.WriteLine("cannot add characters during battle, use 'stop' first");
            return;
        }

        _characters.Add(character);
        _renderer.WriteLine($"{character.Name} added");
    }

    public Character? FindCharacter(string name) => _characters.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public void RegisterAbility(IAbility ability)
    {
        if (_isBattleStarted)
        {
            _renderer.WriteLine("cannot register abilities during battle, use 'stop' first");
            return;
        }

        _abilities.Add(ability);
    }

    public void RegisterItem(IItem item)
    {
        if (_isBattleStarted)
        {
            _renderer.WriteLine("cannot register items during battle, use 'stop' first");
            return;
        }

        _items.Add(item);
    }

    public IAbility? FindAbility(string name) => _abilities.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    public IItem? FindItem(string name) => _items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public void SelectCharacters(string[] names)
    {
        if (_isBattleStarted)
        {
            _renderer.WriteLine("cannot change selection during battle, use 'stop' first");
            return;
        }

        _selectedForBattle.Clear();
        foreach (var name in names)
        {
            var ch = FindCharacter(name);
            if (ch != null)
                _selectedForBattle.Add(ch);
            else
                _renderer.WriteLine($"{name} not found");
        }
        _renderer.WriteLine($"selected for battle: {string.Join(", ", _selectedForBattle.Select(c => c.Name))}");
    }

    public void StartBattle()
    {
        if (_isBattleStarted)
        {
            _renderer.WriteLine("battle is already in progress");
            return;
        }

        if (_selectedForBattle.Count < 2)
        {
            _renderer.WriteLine("need atleast 2 characters, use: select name1 name2");
            return;
        }
        _isBattleStarted = true;
        _renderer.WriteHeader("BATTLE START");
        
        foreach (var c in _selectedForBattle)
            _renderer.WriteLine($"{c.Name} | hp: {c.Health}/{c.MaxHealth} | power: {c.Power} | magic: {c.Magic}");
        _renderer.WriteLine("====================\n");
    }

    public void StopBattle()
    {
        if (!_isBattleStarted)
        {
            _renderer.WriteLine("no battle is currently running");
            return;
        }

        _isBattleStarted = false;
        _renderer.WriteLine("battle stopped");
    }

    private bool CanAct(string actorName, string? targetName = null)
    {
        if (!_isBattleStarted)
        {
            _renderer.WriteLine("battle has not started yet, use 'start' first");
            return false;
        }

        var actor = FindCharacter(actorName);
        if (actor == null || !_selectedForBattle.Contains(actor))
        {
            _renderer.WriteLine($"{actorName} is not in the current battle");
            return false;
        }

        if (targetName != null)
        {
            var target = FindCharacter(targetName);
            if (target == null || !_selectedForBattle.Contains(target))
            {
                _renderer.WriteLine($"{targetName} is not in the current battle");
                return false;
            }
        }
        
        return true;
    }

    public void Attack(string attackerName, string targetName)
    {
        if (!CanAct(attackerName, targetName)) return;

        var attacker = FindCharacter(attackerName);
        var target = FindCharacter(targetName);
        if(attacker == null || target == null)
        {
            _renderer.WriteLine("character not found");
            return;
        }
        attacker.Attack(target);
    }

    public void Heal(string healerName, string targetName, int amount)
    {
        if (!CanAct(healerName, targetName)) return;
        
        var healer = FindCharacter(healerName);
        var target = FindCharacter(targetName);
        if(healer == null || target == null)
        {
            _renderer.WriteLine("character not found");
            return;
        }
        healer.Heal(amount, target);
    }

    public void Defend(string characterName)
    {
        if (!CanAct(characterName)) return;

        var character = FindCharacter(characterName);
        character?.Defend();
    }

    public void UseAbility(string userName, string abilityName, string? targetName = null)
    {
        if (!CanAct(userName, targetName)) return;

        var user = FindCharacter(userName);
        if (user == null) return;

        Character? target = null;
        if (targetName != null) target = FindCharacter(targetName);

        user.UseAbility(abilityName, target);
    }

    public void EquipItem(string characterName, string itemName)
    {
        if (_isBattleStarted)
        {
            _renderer.WriteLine("cannot equip items during battle");
            return;
        }
        var character = FindCharacter(characterName);
        var item = FindItem(itemName);
        if (character == null || item == null)
        {
            _renderer.WriteLine("character or item not found");
            return;
        }
        character.Equip(item);
    }

    public void AddAbilityToCharacter(string characterName, string abilityName)
    {
        if (_isBattleStarted)
        {
            _renderer.WriteLine("cannot add abilities during battle");
            return;
        }
        var character = FindCharacter(characterName);
        var ability = FindAbility(abilityName);
        if (character == null || ability == null)
        {
            _renderer.WriteLine("character or ability not found");
            return;
        }
        character.AddAbility(ability);
    }

    public void ListCharacters()
    {
        if (_characters.Count == 0)
        {
            _renderer.WriteLine("no characters");
            return;
        }
        foreach (var c in _characters)
        {
            string selected = _selectedForBattle.Contains(c) ? " [SELECTED]" : "";
            _renderer.WriteLine($"{c.Name} | health: {c.Health}/{c.MaxHealth} | armor: {c.Armor} | power: {c.Power} | magic: {c.Magic} | alive: {c.IsAlive}{selected}");
        }
    }

    public void ListItems()
    {
        if (_items.Count == 0)
        {
            _renderer.WriteLine("no items");
            return;
        }
        foreach (var i in _items)
            _renderer.WriteLine($"{i.Name} | type: {i.GetType().Name}");
    }

    public void ListAbilities()
    {
        if (_abilities.Count == 0)
        {
            _renderer.WriteLine("no abilities");
            return;
        }
        foreach (var a in _abilities)
            _renderer.WriteLine($"{a.Name} | type: {a.GetType().Name}");
    }
}