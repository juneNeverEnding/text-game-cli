namespace TextGameCLI.Infra.Persistence;

using TextGameCLI.Infra.Persistence.Models;

public interface IRepository
{
    void Save(string name, GameSaveData data);
    GameSaveData? Load(string name);
    GameSaveData? LoadLast();
    IEnumerable<string> ListSaves();
    void Delete(string name);
}