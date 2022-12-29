using UnityEngine;

public static class Constants
{
    public static Vector3 BUILDING_POSITION = new Vector3(0.0f, -4.0f, 62.0f);
    
    public const int LEVELS_AMOUNT = 4;
    
    public const string VFX_LABEL = "VFX";
    public const string GAME_SCENE = "GameScene";
    public const string CAR_SELECTION_SCENE = "CarSelectScene";
    public const string MENU_SCENE = "MenuScene";

    public const string SAVE_DATA_PATH = "/SaveData.json";
    public const string PROJECTILES_LABEL = "Projectile";
    public const string ENEMIES_LABEL = "Enemy";
    
    public const float CUSTOM_GRAVITY = 50.0f;
    public const float LEVEL_OVERVIEW_TIME = 10f;
    public const string FAST_DEVICE_PLAYERPREFS_KEY = "FastDevice";
}