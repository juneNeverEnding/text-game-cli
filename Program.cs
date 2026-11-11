using TextGameCLI.Core;
using TextGameCLI.Infra;
using TextGameCLI.Application;
using TextGameCLI.CLI;
using TextGameCLI.CLI.Strategies;

class Program
{
    static void Main(string[] args)
    {
        IRenderer renderer = new ConsoleRenderer();

        var gameManager = new GameManager(renderer);

        var cli = new CliFacade(renderer);
        cli.UseStrategy(new GameCommandStrategy(gameManager));

        renderer.WriteHeader("text game CLI");
        renderer.WriteLine("type 'help' to see available commands");
        renderer.WriteLine("type 'exit' to quit\n");

        while (true)
        {
            Console.Write("game> ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;
            
            input = input.Trim();

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;
            
            var result = cli.Execute(input);

            if (!string.IsNullOrEmpty(result.Message))
                renderer.WriteLine(result.Message);
        };
        renderer.WriteLine("exiting");
    }
}