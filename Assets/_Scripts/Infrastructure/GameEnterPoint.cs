using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using GameAnalyticsSDK;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameEnterPoint : MonoBehaviour
{
    [Inject] private ISaveManager _saveManager;
    [Inject] private IUserManager _userManager;
    [Inject] private IFacebookManager _facebookSDKManager;

    private void Awake()
    {
        Init();
    }

    private async void Init()
    {
        Application.targetFrameRate = 60;
        var saveData = await _saveManager.Load();
        await _userManager.Init(saveData.UserData);

        var user = _userManager.CurrentUser;
        if (user.Sessions == 0) user.RegistrationDate = DateTime.Now;
        user.Days = (int) Math.Abs((user.RegistrationDate - DateTime.Now).TotalDays);
        user.Sessions++;
        
        _saveManager.Save();
        _facebookSDKManager.FBInit();
        GameAnalytics.Initialize();
        StartCoroutine(SendGameStartEventRoutine());
    }

    private IEnumerator SendGameStartEventRoutine()
    {
        var eventParams = new Dictionary<string, object>();
        eventParams["count"] = _userManager.CurrentUser.Sessions;
        eventParams["days_since_reg"] = _userManager.CurrentUser.Days;
        
        AppMetrica.Instance.ReportEvent("game_start", eventParams);

        GameAnalytics.NewDesignEvent("game_start", 0.0f, eventParams);
        
        while (!FB.IsInitialized) yield return null;
        FB.LogAppEvent("game_start", parameters:eventParams);
        
        yield return null;
        
        SceneManager.LoadScene(Constants.MENU_SCENE);
    }
}