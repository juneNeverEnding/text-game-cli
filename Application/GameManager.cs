namespace TextGameCLI.Application;

using TextGameCLI.Core;
using TextGameCLI.Core.Battle;
using TextGameCLI.Core.Characters;
using TextGameCLI.Core.Combat;

public class GameManager(IRenderer renderer)
{
    private readonly IRenderer _renderer = renderer;
    private readonly List<Character> _characters = [];
    private readonly List<IAbility> _abilities = [];
    private readonly List<IItem> _items = [];
    private readonly List<Character> _selectedForBattle = [];
    private Battle? _currentBattle;

    public IReadOnlyList<Character> Characters => _characters.AsReadOnly();
    public IReadOnlyList<Character> Selected => _selectedForBattle.AsReadOnly();
    public bool IsBattleStarted => _currentBattle?.IsActive ?? false;

    public void AddCharacter(Character character)
    {
        if (IsBattleStarted)
        {
            _renderer.WriteLine("cannot add characters during battle, use 'stop' first");
            return;
        }

        _characters.Add(character);
        _renderer.WriteLine($"{character.Name} added");
    }

    public void RegisterAbility(IAbility ability)
    {
        if (IsBattleStarted)
        {
            _renderer.WriteLine("cannot register abilities during battle, use 'stop' first");
            return;
        }

        _abilities.Add(ability);
    }

    public void RegisterItem(IItem item)
    {
        if (IsBattleStarted)
        {
            _renderer.WriteLine("cannot register items during battle, use 'stop' first");
            return;
        }

        _items.Add(item);
    }

    public Character? FindCharacter(string name) => _characters.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    public IAbility? FindAbility(string name) => _abilities.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    public IItem? FindItem(string name) => _items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public void SelectCharacters(string[] names)
    {
        if (IsBattleStarted)
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
        if (IsBattleStarted)
        {
            _renderer.WriteLine("battle is already in progress");
            return;
        }

        if (_selectedForBattle.Count < 2)
        {
            _renderer.WriteLine("need atleast 2 characters, use: select name1 name2");
            return;
        }
        
        _currentBattle = new Battle();
        _currentBattle.Start(_selectedForBattle);

        _renderer.WriteHeader("BATTLE START");
        
        foreach (var c in _selectedForBattle)
            _renderer.WriteLine($"{c.Name} | hp: {c.Health}/{c.MaxHealth} | power: {c.Power} | magic: {c.Magic}");
        _renderer.WriteLine("====================\n");
    }

    public void StopBattle()
    {
        if (!IsBattleStarted)
        {
            _renderer.WriteLine("no battle is currently running");
            return;
        }

        _currentBattle?.Stop();
        _renderer.WriteLine("battle stopped");
    }

    private bool CanAct(string actorName, string? targetName = null)
    {
        if (!IsBattleStarted)
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

        if (!actor.IsAlive)
        {
            Console.WriteLine($"{actorName} is defeated and cannot act");
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
            if (!target.IsAlive)
            {
                _renderer.WriteLine($"cannot target defeated character {target.Name}");
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
        
        int damage = DamageCalculator.CalculatePhysical(attacker, target);
        string desc = $"{attackerName} attacks {targetName} for {damage} dmg";
        attacker.Attack(target);
        _currentBattle?.Record(attackerName, "attack", targetName, damage, desc);
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

        string desc = $"{healerName} heals {targetName} for {amount}";
        healer.Heal(amount, target);
        _currentBattle?.Record(healerName, "heal", targetName, amount, desc);
    }

    public void Defend(string characterName)
    {
        if (!CanAct(characterName)) return;

        var character = FindCharacter(characterName);
        character?.Defend();

        string desc = $"{characterName} takes a defensive stance";
        _currentBattle?.Record(characterName, "defend", null, 0, desc);
    }

    public void UseAbility(string userName, string abilityName, string? targetName = null)
    {
        if (!CanAct(userName, targetName)) return;

        var user = FindCharacter(userName);
        if (user == null) return;

        Character? target = null;
        if (targetName != null) target = FindCharacter(targetName);

        IAbility? ability = null;
        if (abilityName != null) ability = FindAbility(abilityName);

        int value = ability!.Execute(user, target);
        string desc = $"{userName} used {abilityName}" + (target != null ? $"on {targetName}" : "");
        _currentBattle?.Record(userName, "Ability", targetName, value, desc);
    }

    public void EquipItem(string characterName, string itemName)
    {
        if (IsBattleStarted)
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
        _renderer.WriteLine($"item '{itemName}' added to {characterName}");
    }

    public void AddAbilityToCharacter(string characterName, string abilityName)
    {
        if (IsBattleStarted)
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
        _renderer.WriteLine($"ability '{abilityName}' added to {characterName}");
    }

    public void ShowHistory()
    {
        if (_currentBattle == null || _currentBattle.History.Count == 0)
        {
            _renderer.WriteLine("no battle history yet");
            return;
        }

        _renderer.WriteHeader("BATTLE HISTORY");
        foreach (var turn in _currentBattle.History)
            _renderer.WriteLine(turn.ToString());
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
            _renderer.WriteLine($"{c.Name} | health: {c.Health}/{c.MaxHealth} | armor: {c.Armor} | power: {c.Power} | magic: {c.Magic} | resistance: {c.Resistance} | alive: {c.IsAlive}{selected}");
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