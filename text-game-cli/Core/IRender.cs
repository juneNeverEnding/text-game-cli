namespace TextGameCLI.Core;

public interface IRenderer
{
    void WriteLine(string text = "");
    void Write(string text);
    void Clear();
    void WriteHeader(string text);
}