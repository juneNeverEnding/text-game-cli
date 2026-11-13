# Text Game CLI

Console-based text game made using C# and .NET  
Create characters, get them items and abilities, select the characters and start the battle  

---

## Features

- Create characters, give them a name and assign values to their stats (health, armor, power, magic, resistance)
- Create items and abilities
- Add items and abilities to characters
- Select characters for the battle
- Start and stop battles
- Turn based combat system (`act` command)
- Battle history (`history` command)

---

## Used design patterns

| Pattern              | Usage                                                      |
|----------------------|------------------------------------------------------------|
| Command              | All user actions are performed using commands              |
| Strategy             | `GameCommandStrategy` groups the commands                  |
| Mediator             | `GameManager` controls the interactions between characters |
| Facade               | `CliFacade` provides a simple interface for the CLI system |
| Dependency Injection | Dependencies are passed through constructors               |

---

## Project structure

```text
text-game-cli/
|---Application/      # has GameManager
|---CLI/
|   |---Commands/     # command classes
|   |---Strategies/   # command grouping
|   |---CliFacade.cs  # CLI entry point
|---Core/
|   |---Battle/       # battle, turns
|   |---Characters/   # domain models of characters, abilities, items
|   |---Combat/       # has DamageCalculator, which has formulas for physical and magic damage
|   |--- ...          # interfaces and base classes
|---Infra/            # infrastructure, for now has just ConsoleRenderer.cs
|---Program.cs
|---text-game-cli.csproj
```

---

## Combat formulas

### Physical damage

```text
damage = max(0, Power - effectiveArmor)
effectiveArmor = Armor * 2 (if the target is defending)
effectiveArmor = armor     (if the target is not defending)
```

### Magic damage

```text
raw     = Magic * 1.45
damage  = max(1, raw * (1 - Resistance))
```
`Resistance` is a value between 0.0 and 0.9

---

## Commands

| Command                                       | Description                           |
|-----------------------------------------------|---------------------------------------|
| `create char`                                 | Creates a character                   |
| `create item`                                 | Creates an item                       |
| `create ability`                              | Creates an ability                    |
| `add --char <name> --item <name>`             | Equips an item on a character         |
| `add --char <name> --ability <name>`          | Adds an ability to a character        |
| `ls chars`                                    | Lists all created characters          |
| `ls items`                                    | Lists all created items               |
| `ls abilities`                                | Lists all created abilities           |
| `select <char1> <char2> ...`                  | Selects characters for the battle     |
| `start`                                       | Starts the battle                     |
| `stop`                                        | Stops the battle                      |
| `act attack <actor> <target>`                 | Character attacks a target            |
| `act ability <actor> <target> --id <ability>` | Character uses an ability on a target |
| `act heal <actor> <target>`                   | Character heals a target              |
| `act defend <actor>`                          | Character takes a defensive stance    |
| `history`                                     | Shows current battle history          |
| `help`                                        | Shows available commands              |
| `exit`                                        | Exits the CLI                         |

---

## How to run

```bash
dotnet restore
dotnet run
```