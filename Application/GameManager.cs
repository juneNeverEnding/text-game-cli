namespace TextGameCLI.Application;

using TextGameCLI.Core;
using TextGameCLI.Core.Battle;
using TextGameCLI.Core.Characters;
using TextGameCLI.Core.Combat;
using TextGameCLI.Core.Memento;
using TextGameCLI.Infra.Persistence;
using TextGameCLI.Infra.Persistence.Models;

public class GameManager(IRenderer renderer, IRepository saves)
{
    private readonly IRenderer _renderer = renderer;
    private readonly IRepository _saves = saves;
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
            _renderer.WriteLine($"{actorName} is defeated and cannot act");
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

    public IMemento CreateMemento(string name)
    {
        return new GameMemento(
            name,
            [.. _characters.Select(c => new CharacterData
            (
                c.Name,
                c.MaxHealth,
                c.Health,
                c.Armor,
                c.Power,
                c.Magic,
                c.Resistance,
                c.IsAlive,
                [.. c.Abilities.Select(a => a.Name)],
                [.. c.Inventory.Select(i => i.Name)]
            ))],
            [.. _abilities.Select(a => new AbilityData
            (
                a.Name,
                a.GetType().Name
            ))],
            [.. _items.Select(i => new ItemData
            (
                i.Name,
                i.GetType().Name
            ))],
            [.. _selectedForBattle.Select(c => c.Name)],
            IsBattleStarted,
            _currentBattle?.History.Select(t => new TurnData
            (
                t.Number,
                t.Actor,
                t.Action,
                t.Target,
                t.Value,
                t.Description
            )).ToList() ?? []
        );
    }

    public void RestoreMemento(IMemento memento)
    {
        if (memento is not GameMemento game)
        {
            _renderer.WriteLine("invalid memento type");
            return;
        }

        _characters.Clear();
        _abilities.Clear();
        _items.Clear();
        _selectedForBattle.Clear();
        _currentBattle = null;

        foreach (var a in game.Abilities)
        {
            var ab = CreateAbility(a.Type);
            if (ab != null) _abilities.Add(ab);
        }

        foreach (var i in game.Items)
        {
            var item = CreateItem(i.Type);
            if (item != null) _items.Add(item);
        }

        foreach (var cd in game.Characters)
        {
            var ch = new Character(cd.Name, cd.MaxHealth, cd.Armor, cd.Power, cd.Magic, cd.Resistance);

            if (cd.Health < cd.MaxHealth)
                ch.TakeDamage(cd.MaxHealth - cd.Health);
            
            if (!cd.IsAlive && ch.IsAlive)
                ch.TakeDamage(ch.Health);
            
            foreach (var abName in cd.AbilityNames)
            {
                var ab = FindAbility(abName);
                if (ab != null) ch.AddAbility(ab);
            }

            foreach (var itemName in cd.ItemNames)
            {
                var item = FindItem(itemName);
                if (item != null) ch.AddItem(item);
            }
            _characters.Add(ch);
        }

        foreach (var name in game.SelectedCharacterNames)
        {
            var ch = FindCharacter(name);
            if (ch != null) _selectedForBattle.Add(ch);
        }

        if (game.IsBattleActive && _selectedForBattle.Count >= 2)
        {
            _currentBattle = new Battle();
            _currentBattle.Start(_selectedForBattle);
            foreach (var t in game.BattleHistory)
                _currentBattle.Record(t.Actor, t.Action, t.Target, t.Value, t.Description);
        }
        
        _renderer.WriteLine($"state restored from memento {game.Name}");
    }

    public void Save(string saveName)
    {
        var memento = (GameMemento)CreateMemento(saveName);

        var save = new GameSaveData
        {
            SaveName = saveName,
            Characters = memento.Characters,
            Abilities = memento.Abilities,
            Items = memento.Items,
            SelectedCharacterNames = memento.SelectedCharacterNames,
            IsBattleActive = memento.IsBattleActive,
            BattleHistory = memento.BattleHistory
        };

        _saves.Save(saveName, save);
        _renderer.WriteLine($"saved as {saveName}");
    }

    public void Load(string? saveName)
    {
        GameSaveData? save = string.IsNullOrWhiteSpace(saveName)
            ? _saves.LoadLast()
            : _saves.Load(saveName);

        if (save is null)
        {
            _renderer.WriteLine(saveName is null ? "no last save found" : $"save {saveName} not found");
            return;
        }

        var memento = new GameMemento(
            save.SaveName,
            save.Characters,
            save.Abilities,
            save.Items,
            save.SelectedCharacterNames,
            save.IsBattleActive,
            save.BattleHistory
        );

        RestoreMemento(memento);
    }

    public void ListSaves()
    {
        var names = _saves.ListSaves().ToList();
        if (names.Count == 0)
        {
            _renderer.WriteLine("no saves");
            return;
        }

        foreach (var n in names)
            _renderer.WriteLine(n);
    }

    private static IAbility? CreateAbility(string type) => type switch
    {
        nameof(Fireball) => new Fireball(),
        nameof(LightningBolt) => new LightningBolt(),
        nameof(Heal) => new Heal(),
        _ => null
    };

    private static IItem? CreateItem(string type) => type switch
    {
        nameof(Sword) => new Sword(),
        nameof(Staff) => new Staff(),
        nameof(Armor) => new Armor(),
        nameof(WarriorArmor) => new WarriorArmor(),
        nameof(Robes) => new Robes(),
        nameof(HealthPotion) => new HealthPotion(),
        nameof(StrengthPotion) => new StrengthPotion(),
        nameof(MagicPotion) => new MagicPotion(),
        nameof(ResistancePotion) => new ResistancePotion(),
        _ => null
    };

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