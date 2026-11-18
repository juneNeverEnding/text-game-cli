namespace TextGameCLI.Infra.Persistence;

using System.Text.Json;
using TextGameCLI.Infra.Persistence.Models;

public class JsonRepository : IRepository
{
    private readonly string _savesDir;
    private readonly string _lastPath;

    private static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonRepository()
    {
        var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextGameCLI", "saves");

        Directory.CreateDirectory(root);
        _savesDir = root;
        _lastPath = Path.Combine(root, "_last.json");
    }

    private string PathFor(string name) => Path.Combine(_savesDir, $"{Sanitize(name)}.json");

    private static string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name.Trim();
    }

    public void Save(string name, GameSaveData data)
    {
        data.SaveName = name;
        data.SavedAt = DateTime.UtcNow;

        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(PathFor(name), json);
        File.WriteAllText(_lastPath, json);
    }

    public GameSaveData? Load(string name)
    {
        var path = PathFor(name);
        if (!File.Exists(path)) return null;

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GameSaveData>(json, options);
    }

    public GameSaveData? LoadLast()
    {
        if (!File.Exists(_lastPath)) return null;

        var json = File.ReadAllText(_lastPath);
        return JsonSerializer.Deserialize<GameSaveData>(json, options);
    }

    public IEnumerable<string> ListSaves()
    {
        return Directory.EnumerateFiles(_savesDir, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => n is not null && n != "_last")
            .Cast<string>()
            .OrderBy(n => n);
    }

    public void Delete(string name)
    {
        var path = PathFor(name);
        if (File.Exists(path))
            File.Delete(path);
    }
}