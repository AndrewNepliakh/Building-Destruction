using System;
using System.Collections.Generic;

[Serializable]
public class UserProgress
{
    public LevelData[] Levels = new LevelData[Constants.LEVELS_AMOUNT];

    public UserProgress()
    {
        for (var i = 0; i < Constants.LEVELS_AMOUNT; i++)
        {
            Levels[i] = new LevelData();
        }
    }
}