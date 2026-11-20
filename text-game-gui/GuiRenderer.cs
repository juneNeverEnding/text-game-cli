namespace TextGameGUI;

using System;
using System.Text;
using TextGameCLI.Core;

public class GuiRenderer(Action<string> append) : IRenderer
{
    private readonly Action<string> _append = append;
    private readonly StringBuilder _buffer = new();

    public void Write(string text)
    {
        _buffer.Append(text);
        Flush();
    }

    public void WriteLine(string text = "")
    {
        _buffer.AppendLine(text);
        Flush();
    }

    public void WriteHeader(string text)
    {
        _buffer.AppendLine();
        _buffer.AppendLine("=== " + text + " ===");
        _buffer.AppendLine();
        Flush();
    }

    public void Clear()
    {
        _buffer.Clear();
        _append("\u0000CLEAR\u0000");
    }

    private void Flush()
    {
        if (_buffer.Length == 0) return;
        var chunk = _buffer.ToString();
        _buffer.Clear();
        _append(chunk);
    }
}