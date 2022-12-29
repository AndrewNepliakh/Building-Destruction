using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


public class ScenesToolUtility
{
    [MenuItem("Scenes/Init")]
    public static void InitScene() => OpenEditorScene("InitScene");

    [MenuItem("Scenes/Game")]
    public static void GameScene() => OpenEditorScene("GameScene");

    [MenuItem("Scenes/TutorialScene")]
    public static void TutorialScene() => OpenEditorScene("TutorialScene");

    [MenuItem("Scenes/MenuScene")]
    public static void MenuScene() => OpenEditorScene("MenuScene");

    [MenuItem("Scenes/CarSelectScene")]
    public static void CarSelectScene() => OpenEditorScene("CarSelectScene");


    static void OpenEditorScene(string sceneName)
    {
        if (Application.isPlaying)
            return;

        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/" + sceneName + ".unity");
    }
}