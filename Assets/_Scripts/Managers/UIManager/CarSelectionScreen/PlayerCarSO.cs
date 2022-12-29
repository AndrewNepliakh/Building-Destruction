using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Scriptable Objects/Cars/New Car")]
public class PlayerCarSO : ScriptableObject
{
    public string Name;
    public int Price;
    public AssetReferenceGameObject CarModelPrefab;
    public AssetReferenceGameObject InGamePrefab;
}