using System.Threading.Tasks;
using Managers;
using RayFire;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameSceneController : GameSceneStateMachine
{
    [Inject] private IUIManager _uiManager;
    [Inject] private ILevelManager _levelManager;
    [Inject] private ISoundManager _soundManager;
    [Inject] private IUserManager _userManager;
    [Inject] private ISaveManager _saveManager;

    [Inject] private SignalBus _signalBus;
    private MainHUDWindow _mainHUDWindow;
    private CommonPopup _commonPopup;
    private TouchInput _touchInput;
    
    [SerializeField] private LevelCameraController _cameraController;
    [SerializeField] private RayFireManManager _rayFireManManager;
    
    public bool IsEnd { get; private set; }
    private int _gameStarsCount = 0;

    [SerializeField] private PlayerSpawner _playerSpawner;
    private ICarController _carController;
    
    private void Awake()
    {
        _touchInput = GetComponent<TouchInput>();
        _touchInput.OnTouchEnded += Next;
        _playerSpawner.OnPlayerSpawned += OnPlayerSpawned;
    }

    private void OnPlayerSpawned(ICarController carController)
    {
        _carController = carController; 
        _carController.OnSpeedUpdate += _mainHUDWindow.UpdateSpeedometr;
        _mainHUDWindow.Speedometr.SetActive(true);

    }

    private async void Start()
    {
        _rayFireManManager.Init();

        _levelManager.OnLevelLoaded += OnLevelLoaded;

        _mainHUDWindow = await _uiManager.ShowWindowWithDI<MainHUDWindow>();
        _mainHUDWindow.OnTapPlay += OnTapPlay;
        
        await _levelManager.CreateLevel((LevelID) _userManager.CurrentUser.CurrentLevelIndex);

        _signalBus.Subscribe<GameCrushedSignal>(OnLevelEnd);
        _signalBus.Subscribe<GameAttemptEndedSignal>(EndAttempt);
        _signalBus.Subscribe<OnCarTookOff>(TakeOff);
        _signalBus.Subscribe<OnCarFallOff>(FallFromPlatform);
    }

    private void OnLevelLoaded(ILevel level)
    {
        _cameraController.Init(level.transform.GetComponentsInChildren<ICameraController>());
        level.OnAttemptChanged += _mainHUDWindow.UpdateTryCars;
    }


    private void OnLevelEnd(GameCrushedSignal result)
    {
        _gameStarsCount = result.Stars;
        IsEnd = true;
        EndGame();
        OnGameEnd();
    }

    private void OnMenuTapped()
    {
        UnloadAll();
        _saveManager.Save();
        SceneManager.LoadScene(Constants.MENU_SCENE);
    }

    private void UnloadAll()
    {
        _levelManager.UnloadLevels();
    }

    private void OnDestroy()
    {
        _carController.OnSpeedUpdate -= _mainHUDWindow.UpdateSpeedometr;
        _levelManager.CurrentLevel.OnAttemptChanged -= _mainHUDWindow.UpdateTryCars;
        _mainHUDWindow.OnTapPlay -= OnTapPlay;
        _commonPopup.OnMenuTapped -= OnMenuTapped;
        _touchInput.OnTouchEnded -= Next;
        _commonPopup.OnNextLevel -= OnNextLevel;
        _levelManager.OnLevelLoaded -= OnLevelLoaded;
        
        _signalBus.Unsubscribe<GameCrushedSignal>(OnLevelEnd);
        _signalBus.Unsubscribe<GameAttemptEndedSignal>(EndAttempt);
        _signalBus.Unsubscribe<OnCarTookOff>(TakeOff);
        _signalBus.Unsubscribe<OnCarFallOff>(FallFromPlatform);
    }

    private void OnTapPlay() => StartGame();

    public void OnGameStart()
    {
        IsEnd = false;
        _soundManager.PlayAudioSource<LevelStartSoundSource>();
        _mainHUDWindow.HideTapPlayButton();
        _signalBus.TryFire<GameStartedSignal>();
    }

    private void OnRestart()
    {
        _rayFireManManager.Init();
        NextLevel();
        _levelManager.CurrentLevel.Init(null);
        _signalBus.Fire<GameRestartSignal>();
        CommonPopupEventsUnsubscribe();
    }

    private void OnGameEnd()
    {
        _signalBus.Fire<GameEndSignal>();
    }

    private void OnNextLevel()
    {
        _rayFireManManager.Init();
        NextLevel();
        CommonPopupEventsUnsubscribe();

        _levelManager.CurrentLevel.OnAttemptChanged -= _mainHUDWindow.UpdateTryCars;

        _levelManager.NextLevel();
        _signalBus.Fire<GameRestartSignal>();
        _saveManager.Save();
        _mainHUDWindow.Speedometr.SetActive(true);
    }

    private void CommonPopupEventsUnsubscribe()
    {
        _commonPopup.OnNextLevel -= OnNextLevel;
        _commonPopup.OnMenuTapped -= OnMenuTapped;
        _commonPopup.OnRestartTapped -= OnRestart;
    }

    public async void ShowEndGameWindow()
    {
        await Task.Delay(1500);

        _commonPopup = await _uiManager.ShowPopupWithDI<CommonPopup>(
            new UIViewArguments
            {
                Stars = _gameStarsCount
            });

        _commonPopup.OnNextLevel += OnNextLevel;
        _commonPopup.OnMenuTapped += OnMenuTapped;
        _commonPopup.OnRestartTapped += OnRestart;

        _mainHUDWindow.Speedometr.SetActive(false);

        if (_userManager.CurrentUser.UserProgress.Levels[(int) _levelManager.CurrentLevel.LevelID].Stars <
            _gameStarsCount)
        {
            _userManager.CurrentUser.UserProgress.Levels[(int) _levelManager.CurrentLevel.LevelID].Stars =
                _gameStarsCount;
        }

        switch (_gameStarsCount)
        {
            case 1:
                _userManager.CurrentUser.Currency += 1000;
                break;
            case 2:
                _userManager.CurrentUser.Currency += 1500;
                break;
            case 3:
                _userManager.CurrentUser.Currency += 2000;
                break;
        }
    }

    public void StartAttempt()
    {
        _signalBus.TryFire<GameAttemptStartedSignal>();
    }

    public void OnBeforeStart()
    {
        _signalBus.Fire<OnBeforeGameStartSignal>();
    }
}