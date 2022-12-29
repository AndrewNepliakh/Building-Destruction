using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class SaveManager : ISaveManager
{
    [Inject] private IUserManager _userManager;
    
    public void Save()
    {
        var saveDataPath = Application.persistentDataPath + Constants.SAVE_DATA_PATH;

        var saveData = new SaveData()
        {
            UserData = new UserData{User = _userManager.CurrentUser}
        };

        var json = JsonConvert.SerializeObject(saveData);

        if (!File.Exists(saveDataPath))
        {
            var file = File.Create(saveDataPath);
            file.Close();
        }
        
        File.WriteAllText(saveDataPath, json);
    }

    public Task<SaveData> Load()
    {
        var saveDataPath = Application.persistentDataPath + Constants.SAVE_DATA_PATH;

        if (File.Exists(saveDataPath))
        {
            var json = File.ReadAllText(saveDataPath);
            
            SaveData saveData;
            
            try
            {
                saveData = JsonConvert.DeserializeObject<SaveData>(json);
            }
            catch (Exception e)
            {
                saveData = GetDefaultSaveData();
                Debug.LogError($"{e.Message}: SaveData doesn't deserialized properly. Was get default saveData.");
            }

            return Task.FromResult(saveData);
        }

        return Task.FromResult(GetDefaultSaveData());
    }

    public SaveData GetDefaultSaveData()
    {
        return new SaveData
        {
            UserData = new UserData{User = new User()}
        };
    }
}