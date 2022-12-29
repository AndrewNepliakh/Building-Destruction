using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class LevelManager : ILevelManager
    {
        public event Action<ILevel> OnLevelLoaded;
        [Inject] private DiContainer _diContainer;
        [Inject] private IUserManager _userManager;
        [Inject] private SignalBus _signalBus;
        public ILevel CurrentLevel { get; private set; }

        private Transform _parent;

        private int _levelIndex = 0;

        public async Task CreateLevel()
        {
            NextLevel();
        }

        public async Task CreateLevel(LevelID id)
        {
            CurrentLevel?.UnloadLevel();

            var loader = new AssetsLoader();
            CurrentLevel = await loader.InstantiateAssetWithDI<ILevel>(id.ToString(), _diContainer, _parent);
            CurrentLevel.loader = loader;
            CurrentLevel.Init(new object());
            OnLevelLoaded?.Invoke(CurrentLevel);
        }

        public async Task NextLevel()
        {
            _levelIndex = (++_levelIndex % Enum.GetNames(typeof(LevelID)).Length) ;
            await CreateLevel((LevelID)_levelIndex);
        }

        public void UnloadLevels()
        {
            CurrentLevel?.UnloadLevel();
        }

        public async void RestartLevel()
        {
            await CreateLevel((LevelID)_userManager.CurrentUser.CurrentLevelIndex);
        }
    }
}