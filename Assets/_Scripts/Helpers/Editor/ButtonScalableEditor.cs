using Managers;
using UnityEditor;


[CustomEditor(typeof(ButtonScalable))]
public class ButtonScalableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}