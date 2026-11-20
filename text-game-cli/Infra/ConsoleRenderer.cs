namespace TextGameCLI.Infra;

using TextGameCLI.Core;

public class ConsoleRenderer : IRenderer
{
    public void Clear() => Console.Clear();
    public void Write(string text) => Console.Write(text);
    public void WriteLine(string text = "") => Console.WriteLine(text);
    public void WriteHeader(string text)
    {
        Console.WriteLine();
        Console.WriteLine("=== " + text + " ===");
        Console.WriteLine();
    }
}