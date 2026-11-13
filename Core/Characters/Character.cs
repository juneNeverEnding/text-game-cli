namespace TextGameCLI.Core.Characters;

using TextGameCLI.Core.Combat;

/// <summary>
/// character template
/// </summary>
public class Character
{

    //character stats
    public string Name { get; }
    public int MaxHealth { get; private set; }
    public int Health { get; private set; }
    public int Armor { get; private set; }
    public int Power { get; private set; }
    public int Magic { get; private set; }

    public bool IsAlive { get; private set; } = true;
    private readonly List<IAbility> _abilities = [];
    private readonly List<IItem> _inventory = [];
    private bool _isDefending;

    public Character(string name, int health, int armor, int power, int magic)
    {
        //setting stats on the character
        Name = name;
        MaxHealth = Health = health;
        Armor = armor;
        Power = power;
        Magic = magic;
    }

    public IReadOnlyList<IAbility> Abilities => _abilities.AsReadOnly();
    public IReadOnlyList<IItem> Inventory => _inventory.AsReadOnly();
    public bool IsDefending => _isDefending;

    public void Attack(Character target)
    {
        int Damage = DamageCalculator.CalculatePhysical(this, target);
        target.TakeDamage(Damage);
        Console.WriteLine($"{Name} attacks {target.Name} and dealt {Damage} damage\n{target.Name}'s current health: {target.Health}");
        target._isDefending = false; //incase the target is defending
    }

    public void Heal(int amount, Character target)
    {
        target.ReceiveHeal(amount);
        Console.WriteLine($"{Name} heals {target.Name} for {amount}\n{target.Name}'s current health: {target.Health}");
    }

    public void Defend()
    {
        if (!IsAlive) return;
        _isDefending = true;
        Console.WriteLine($"{Name} takes a defensive stance");
    }


    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;
        Health = Math.Max(0, Health - amount);
        if (Health <= 0)
        {
            Console.WriteLine($"\n{Name} has been defeated");
            IsAlive = false;
        }
    }

    public void ReceiveHeal(int amount) //made this a function so it can be used for healing potions without using the heal function
    {
        if (!IsAlive) return;
        Health = Math.Min(MaxHealth, Health + amount);
    }

    public void AddAbility(IAbility ability) => _abilities.Add(ability);

    public void UseAbility(string abilityName, Character? target = null)
    {
        var ability = _abilities.FirstOrDefault(a => a.Name == abilityName);
        if (ability is null)
        {
            Console.WriteLine($"{Name} does not have the ability '{abilityName}'");
            return;
        }
        ability.Execute(this, target);
    }

    public void Equip(IItem item)
    {
        if (!IsAlive) return;
        item.Apply(this);
        _inventory.Add(item);
        Console.WriteLine($"{Name} equips {item.Name}\n");
    }

    internal void AddItem(IItem item) => _inventory.Add(item);

    internal void ModifyStats(int healthDelta = 0, int armorDelta = 0, int powerDelta = 0, int magicDelta = 0)
    {
        if (!IsAlive) return;
        MaxHealth += healthDelta;
        Armor += armorDelta;
        Power += powerDelta;
        Magic += magicDelta;
    }
}