using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Managers
{
    public class MenuSceneController : MonoBehaviour
    {
        [Inject] private IUIManager _uiManager;
        [Inject] private ISoundManager _soundManager;
        [Inject] private IUserManager _userManager;
        [Inject] private ISaveManager _saveManager;

        private MenuWindow _menuWindow;

        private async void Start()
        {
            _menuWindow = await _uiManager.ShowWindowWithDI<MenuWindow>();

            _menuWindow.OnSettingsClicked += OpenSettingsPopup;
            _menuWindow.OnVehicleClicked += ToVehicleSelectScene;
            _menuWindow.OnDailyClicked += OnDailyClicked;
            _menuWindow.OnNoAdsClicked += OnNoAdsClicked;


            foreach (var level in _menuWindow.LevelsButton)
            {
                level.Init(new LevelUIArgs {UserManager = _userManager});
                level.OnSelectLevel += OnSelectLevel;
            }

            SendMenuEventRoutine();
        }

        private void OnSelectLevel(int index)
        {
           
            if(index != 0 && _userManager.CurrentUser.UserProgress.Levels[index - 1].Stars == 0)
            {
                return;
            }
            
            _userManager.CurrentUser.CurrentLevelIndex = index;
            _saveManager.Save();
            GoToScene(Constants.GAME_SCENE);
        }

        private void OpenSettingsPopup()
        {
        }

        private void ToVehicleSelectScene()
        {
            GoToScene(Constants.CAR_SELECTION_SCENE);
        }

        private void OnDailyClicked()
        {
        }

        private void OnNoAdsClicked()
        {
        }

        private void GoToScene(string sceneName)
        {
            _menuWindow.OnSettingsClicked -= OpenSettingsPopup;
            _menuWindow.OnVehicleClicked -= ToVehicleSelectScene;
            _menuWindow.OnDailyClicked -= OnDailyClicked;
            _menuWindow.OnNoAdsClicked -= OnNoAdsClicked;

            foreach (var level in _menuWindow.LevelsButton)
            {
                level.OnSelectLevel -= OnSelectLevel;
            }

            SceneManager.LoadScene(sceneName);
        }

        private void SendMenuEventRoutine()
        {
            var eventParams = new Dictionary<string, object>();
            eventParams["days_since_reg"] = _userManager.CurrentUser.Days;
            
            AppMetrica.Instance.ReportEvent("main_menu", eventParams);
            GameAnalytics.NewDesignEvent("main_menu", 0.0f, eventParams);
            FB.LogAppEvent("main_menu", parameters: eventParams);
        }
    }
}