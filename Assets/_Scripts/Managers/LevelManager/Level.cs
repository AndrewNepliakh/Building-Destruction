using System;
using System.Collections.Generic;
using Facebook.Unity;
using GameAnalyticsSDK;
using Managers;
using UnityEngine;
using Zenject;


public class Level : MonoBehaviour, ILevel
{
    public Action<int> OnAttemptChanged { get; set; }

    public LevelID LevelID => _levelId;
    [SerializeField] private LevelID _levelId;
    [SerializeField] private Transform _levelSpawnPoint;

    [Inject] private SignalBus _signalBus;
    [Inject] private IBuildingManager _buildingManager;
    [Inject] private IUserManager _userManager;
    private int _currentAttempt = 0;
    [field: SerializeField] public int MaxAttempts { get; private set; } = 3;
    [SerializeField] private BuildingID _buildingID;
    private IBuilding _building;
    public AssetsLoader loader { get; set; }

    public Vector3 SpawnPoint => _levelSpawnPoint == null ? Vector3.zero : _levelSpawnPoint.position;

    private DateTime _startTime;

    public async void Init(object o)
    {
        _currentAttempt = 0;
        _building = await _buildingManager.CreateBuilding(_buildingID);
        _startTime = DateTime.Now;

        if (_userManager.CurrentUser.CurrentLevelIndex == (int) _levelId)
        {
            SendRestartEvent();
        }
        else
        {
            SendStartEvent();
        }
    }

    private void Start()
    {
        _signalBus.Subscribe<OnLevelComplete>(SendCompleteEvent);
        _signalBus.Subscribe<OnLevelFailed>(SendFailedEvent);
    }

    public void UnloadLevel()
    {
        _building?.loader.UnloadAsset();
        loader?.UnloadAsset();
    }

    private void Awake()
    {
        _signalBus.Subscribe<GameAttemptEndedSignal>(OnAttemptEnded);
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<OnLevelComplete>(SendCompleteEvent);
        _signalBus.Unsubscribe<OnLevelFailed>(SendFailedEvent);
        _signalBus.Unsubscribe<GameAttemptEndedSignal>(OnAttemptEnded);
    }

    private void OnAttemptEnded()
    {
        _currentAttempt++;
        OnAttemptChanged?.Invoke(_currentAttempt);
        if (_currentAttempt >= MaxAttempts)
            _signalBus.Fire(new GameCrushedSignal {Stars = _building.GetLevelResult()});
    }

    private void SendStartEvent()
    {
        var eventParams = new Dictionary<string, object>();
        eventParams["level"] = ((int) _levelId) + 1;
        eventParams["days_since_reg"] = _userManager.CurrentUser.Days;
        
        SendEvent("level_start", eventParams);
    }

    private void SendRestartEvent()
    {
        var eventParams = new Dictionary<string, object>();
        eventParams["level"] = ((int) _levelId) + 1;
        eventParams["days_since_reg"] = _userManager.CurrentUser.Days;

        SendEvent("level_restart", eventParams);
    }

    private void SendCompleteEvent()
    {
        var eventParams = new Dictionary<string, object>();
        eventParams["level"] = ((int) _levelId) + 1;
        eventParams["time_spent"] = (int) Math.Abs((_startTime - DateTime.Now).TotalSeconds);
        eventParams["days_since_reg"] = _userManager.CurrentUser.Days;

        SendEvent("level_complete", eventParams);
    }
    
    private void SendFailedEvent()
    {
        var eventParams = new Dictionary<string, object>();
        eventParams["level"] = ((int) _levelId) + 1;
        eventParams["reason"] = "Not enough level of demolition";
        eventParams["time_spent"] = (int) Math.Abs((_startTime - DateTime.Now).TotalSeconds);
        eventParams["days_since_reg"] = _userManager.CurrentUser.Days;

        SendEvent("level_fail", eventParams);
        Debug.Log("level_fail");
    }

    private void SendEvent(string eventName, Dictionary<string, object> parameters)
    {
        AppMetrica.Instance.ReportEvent(eventName, parameters);
        GameAnalytics.NewDesignEvent(eventName, 0.0f, parameters);
        FB.LogAppEvent(eventName, parameters: parameters);
    }
}