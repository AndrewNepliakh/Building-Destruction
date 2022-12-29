using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[Serializable]
public class User : IUser
{
    public int Currency;
    public int CurrentLevelIndex;
    public UserProgress UserProgress;
    public bool[] Cars;

    public int Sessions; // total number of started sessions
    public DateTime RegistrationDate;
    public int Days; // total number of days since registration

    public User()
    {
        UserProgress = new UserProgress();
        Currency = 0;
        
        // var settings =
            // (GameSettingsSO) AssetDatabase.LoadAssetAtPath("Assets/_Sos/DefaultSettings.asset", typeof(GameSettingsSO));
        Cars = new bool[7];
        Cars[0] = true;

        Sessions = 0;
        Days = 0;
    }

    public void ResetCars()
    {
        for (var i = 0; i < Cars.Length; i++)
        {
            if (i == 0)
            {
                Cars[i] = true;
            }
            else
            {
                Cars[i] = false;
            }
        }
    }
}