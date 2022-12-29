using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Scriptable objects/GameSetting")]
public class GameSettingsSO : ScriptableObject, IGameSettingsEditor
{
    public int CurrentCar { get; private set; } = 0;
    [field: SerializeField] public CarsSetSO Cars { get; private set; }

    void IGameSettingsEditor.SelectCar(int carIndex) => CurrentCar = carIndex;

    public AssetReferenceGameObject GetSelectedPlayerAsset() => Cars.Cars[CurrentCar].InGamePrefab;
}