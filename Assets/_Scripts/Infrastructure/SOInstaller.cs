using Managers;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "SOInstaller", menuName = "Installers/SOInstaller")]
public class SOInstaller : ScriptableObjectInstaller<SOInstaller>
{
    [SerializeField] private GameSettingsSO _settings;
    public override void InstallBindings()
    {
        Container.BindInstance(_settings);
    }
}