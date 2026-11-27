# Text Game

A C# / .NET text game with an Avalonia GUI and CLI  
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
- Save and load the game (`save <name>` and `load [name]` commands)
- Avalonia graphical interface
- Command line interface

---

## Projects

- `text-game-cli` has the game functional, commands, persistence, CLI
- `text-game-gui` is the avalonia graphical interface

---

## Used design patterns

| Pattern              | Usage                                                      |
|----------------------|------------------------------------------------------------|
| Command              | All user actions are performed using commands              |
| Strategy             | `GameCommandStrategy` groups the commands                  |
| Mediator             | `GameManager` controls the interactions between characters |
| Facade               | `CliFacade` provides a simple interface for the CLI system |
| Memento              | `GameManager` creates/restores `GameMemento` snapshots     |
| Repository           | `JsonRepository` persists saves to disk                    |
| Dependency Injection | Dependencies are passed through constructors               |

---

## Project structure

```text
text-game-cli/
|---Application/            # has GameManager
|---CLI/
|   |---Commands/           # command classes
|   |---Strategies/         # command grouping
|   |---CliFacade.cs        # CLI entry point
|---Core/
|   |---Battle/             # battle, turns
|   |---Characters/         # domain models of characters, abilities, items
|   |---Combat/             # has DamageCalculator, which has formulas for physical and magic damage
|   |---Memento/            # IMemento, GameMemento
|   |--- ...                # interfaces and base classes
|---Infra/
|   |---ConsoleRenderer.cs
|   |---Persistence/
|   |   |---Models/         # models for saving data
|   |   |---IRepository.cs  
|   |   |---JsonRepository.cs
|---Program.cs
|---text-game-cli.csproj

text-game-gui/
|---App.axaml
|---App.axaml.cs
|---app.manifest
|---GuiRenderer.cs          # gui rendering
|---MainWindow.axaml        # main gui
|---MainWindow.axaml.cs     # gui code
|---Program.cs
|---text-game-gui.csproj
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

## Persistence

Game state is serialized to JSON using `System.Text.Json`  

Saves are located in:  
`%LocalAppData%\TextGameCLI\saves\` (Windows)  
`~/.local/share/TextGameCLI/saves/` (Linux)  

There are named saves, and there's also the last save which is called `_last.json`, it updates on every `save`  

### What is stored

- All characters, their stats, HP, whether if they're alive or not
- Abilities and items
- Which abilities or items each character has
- Selected battle roster
- Whether if there's a battle ongoing
- Battle history

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
| `ls saves`                                    | Lists all saves                       |
| `select <char1> <char2> ...`                  | Selects characters for the battle     |
| `start`                                       | Starts the battle                     |
| `stop`                                        | Stops the battle                      |
| `act attack <actor> <target>`                 | Character attacks a target            |
| `act ability <actor> <target> --id <ability>` | Character uses an ability on a target |
| `act heal <actor> <target>`                   | Character heals a target              |
| `act defend <actor>`                          | Character takes a defensive stance    |
| `history`                                     | Shows current battle history          |
| `save <name>`                                 | Saves the game with a name            |
| `load [name]`                                 | Loads a save, if entered without a name it loads the last save |
| `help`                                        | Shows available commands              |
| `exit`                                        | Exits the CLI                         |

You can also enter `cancel` to stop the creation of a character, item or an ability

---


## Download (release builds)

[GitHub Releases](https://github.com/juneNeverEnding/text-game-cli/releases)

### Requirements
**None** for the releases (self-contained, .NET runtime is included)  
To build from source you need .NET 10 SDK

### How to run

#### Windows

CLI
1. Download `text-game-cli-win-x64.zip`
2. Extract
3. Run `text-game-cli.exe`

GUI
1. Download `text-game-gui-win-x64.zip`
2. Extract
3. Run `text-game-gui.exe`

---

#### Linux

CLI
1. Download `text-game-cli-linux-x64.zip`
2. Extract
3. `chmod +x text-game-cli`
4. `./text-game-cli`

GUI
1. Download `text-game-gui-linux-x64.zip`
2. Extract
3. `chmod +x text-game-gui`
4. `./text-game-gui`


### Build from source
```bash
dotnet restore
dotnet run --project text-game-cli/text-game-cli.csproj
dotnet run --project text-game-gui/text-game-gui.csproj
```