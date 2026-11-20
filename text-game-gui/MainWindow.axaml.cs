using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using TextGameCLI.Application;
using TextGameCLI.CLI;
using TextGameCLI.CLI.Strategies;
using TextGameCLI.Core;
using TextGameCLI.Infra.Persistence;

namespace TextGameGUI;

public partial class MainWindow : Window
{
    private readonly GameManager _manager;
    private readonly CliFacade _cli;
    private readonly IRenderer _renderer;
    public MainWindow()
    {
        InitializeComponent();

        _renderer = new GuiRenderer(AppendOutput);
        IRepository saves = new JsonRepository();
        _manager = new GameManager(_renderer, saves);

        _cli = new CliFacade(_renderer);
        _cli.UseStrategy(new GameCommandStrategy(_manager));

        _renderer.WriteHeader("text game GUI");
        _renderer.WriteLine("type 'help' for commands");
        _renderer.WriteLine("use the saves panel to load a save");
        _renderer.WriteLine();

        RefreshSaves();
    }

    private void AppendOutput(string text)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (text == "\u000CLEAR\u0000")
            {
                OutputBox.Text = "";
                return;
            }

            OutputBox.Text += text;
            OutputScroll.ScrollToEnd();
        });
    }

    private void RunLine(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;
        
        input = input.Trim();
        _renderer.WriteLine($"> {input}");

        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            Close();
            return;
        }

        var result = _cli.Execute(input);
        if (!string.IsNullOrEmpty(result.Message))
            _renderer.WriteLine(result.Message);

        RefreshSaves();
    }

    private void OnRunCommand(object? sender, RoutedEventArgs e)
    {
        var line = CommandBox.Text ?? "";
        CommandBox.Text = "";
        RunLine(line);
    }

    private void OnCommandKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        OnRunCommand(sender, e);
        e.Handled = true;
    }

    private void OnLoadSelected(object? sender, RoutedEventArgs e)
    {
        if (SavesList.SelectedItem is string name)
        {
            _manager.Load(name);
            RefreshSaves();
        }
        else
            _renderer.WriteLine("select a save from the list first");
    }

    private void OnLoadLast(object? sender, RoutedEventArgs e)
    {
        _manager.Load(null);
        RefreshSaves();
    }

    private void OnRefreshSaves(object? sender, RoutedEventArgs e) => RefreshSaves();

    private void RefreshSaves()
    {
        SavesList.Items.Clear();
        foreach (var name in new JsonRepository().ListSaves())
            SavesList.Items.Add(name);
    }
}