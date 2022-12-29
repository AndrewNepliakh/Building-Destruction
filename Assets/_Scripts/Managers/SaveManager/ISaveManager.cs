using System.Threading.Tasks;

public interface ISaveManager
{
    void Save();
    Task<SaveData> Load();
    SaveData GetDefaultSaveData();
}