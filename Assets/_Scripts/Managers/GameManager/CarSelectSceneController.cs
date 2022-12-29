using System.Collections.Generic;
using System.Linq;
using _Scripts.Player;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class CarSelectSceneController : MonoBehaviour
{
    private TouchInput _touchInput;
    [SerializeField] private Transform _camera;
    [SerializeField] private float _cameraMoveSpeed = 1;
    [SerializeField] private float _gridSize = 10;
    [SerializeField] private CarSlotSpawner _slotSpawnerPrefab;
    [SerializeField] private float _swipeDetectionThreshold = 3;

    [Inject] private GameSettingsSO _settings;
    [Inject] private IUIManager _uiManager;
    [Inject] private IUserManager _userManager;
    [Inject] private ISaveManager _saveManager;

    private float _pixelToUnitsRatio = 1;
    private int _currentSlot = 0;
    private int _slotsCount;
    private bool _snapEnabled = true;
    private int _targetSlot = 0;
    private bool _isSwipeDetected = false;
    private bool _isNotEnoughMoneyPopupActive;

    private NotEnoughMoneyPopup _notEnoughMoneyPopup;
    private CarSelectWindow _carSelectWindow;
    private List<ColorController> _carColors = new();

    private async void Awake()
    {
        _touchInput = GetComponent<TouchInput>();
        _touchInput.OnMove += OnSwipe;
        _touchInput.OnTouchEnded += OnTouchEnded;
        _touchInput.OnTouchBegan += OnTouchBegan;
        _pixelToUnitsRatio = (float) Screen.width / 540 / 100;

        float currentXPos = 0;

        for (var i = 0; i < _settings.Cars.Cars.Count; i++)
        {
            Instantiate(_slotSpawnerPrefab, Vector3.right * currentXPos, Quaternion.identity, transform)
                .Init(_settings.Cars.Cars[i].CarModelPrefab, _camera, _carColors, CheckForColorCar, i);
            currentXPos -= _gridSize;
        }

        _carSelectWindow = await _uiManager.ShowWindowWithDI<CarSelectWindow>();
        _carSelectWindow.OnBackToMenu += BackToMenu;
        _carSelectWindow.OnPlayTapped += SelectCarAndLoadGame;
        _carSelectWindow.OnBuyNowButtonClicked += BuyCar;
        _carSelectWindow.BuyButton.Hide();
        CheckForUnlockCar();
    }

    private void OnDestroy()
    {
        _touchInput.OnMove -= OnSwipe;
        _touchInput.OnTouchEnded -= OnTouchEnded;
        _touchInput.OnTouchBegan -= OnTouchBegan;

        _carSelectWindow.OnBackToMenu -= BackToMenu;
        _carSelectWindow.OnPlayTapped -= SelectCarAndLoadGame;
        _carSelectWindow.OnBuyNowButtonClicked -= BuyCar;
    }

    private void Start()
    {
        _slotsCount = _settings.Cars.Count();
    }

    private void CheckForColorCar(int index)
    {
        if (!_userManager.CurrentUser.Cars[index])
        {
            SetCarColorCondition(ColorCondition.TurnGrey, index);
            return;
        }

        SetCarColorCondition(ColorCondition.Reset, index);
    }

    private void CheckForUnlockCar()
    {
        if (!_userManager.CurrentUser.Cars[_targetSlot])
        {
            _carSelectWindow.Cross.SetActive(true);
            _carSelectWindow.BuyButton.Show();
            _carSelectWindow.PlayButton.Hide();
            _carSelectWindow.UpdatePrice(_targetSlot);
            SetCarColorCondition(ColorCondition.TurnGrey, _targetSlot);
            return;
        }

        _carSelectWindow.BuyButton.Hide();
        _carSelectWindow.PlayButton.Show();
        _carSelectWindow.Cross.SetActive(false);
        _carSelectWindow.UpdatePrice(0);
        SetCarColorCondition(ColorCondition.Reset, _targetSlot);
    }

    private void SetCarColorCondition(ColorCondition condition, int index)
    {
        foreach (var color in _carColors)
        {
            if (color.Index != index) continue;
            var colors = color.GetComponentsInChildren<ColorController>();
            foreach (var cl in colors)
            {
                if (condition == ColorCondition.TurnGrey)
                    cl.TurnGreyColor();
                else
                    cl.ResetColor();
            }
        }
    }
    
    private void BackToMenu()
    {
        _saveManager.Save();
        SceneManager.LoadScene(Constants.MENU_SCENE);
    }

    private void SelectCarAndLoadGame()
    {
        IGameSettingsEditor settings = _settings;
        _saveManager.Save();
        settings.SelectCar(_targetSlot);
        SceneManager.LoadScene(Constants.GAME_SCENE);
    }

    private async void BuyCar()
    {
        IGameSettingsEditor settings = _settings;

        if (_userManager.CurrentUser.Currency >= settings.Cars.Cars[_targetSlot].Price)
        {
            _userManager.CurrentUser.Currency -= settings.Cars.Cars[_targetSlot].Price;
            _userManager.CurrentUser.Cars[_targetSlot] = true;
            settings.SelectCar(_targetSlot);
            CheckForUnlockCar();
        }
        else
        {
            var difference = settings.Cars.Cars[_targetSlot].Price - _userManager.CurrentUser.Currency;
            _isNotEnoughMoneyPopupActive = true;
            _notEnoughMoneyPopup =
                await _uiManager.ShowPopupWithDI<NotEnoughMoneyPopup>(new UIViewArguments {Difference = difference});
            _notEnoughMoneyPopup.OnClose += CloseNotEnoughMoneyPopup;
        }
    }

    private void CloseNotEnoughMoneyPopup()
    {
        _isNotEnoughMoneyPopupActive = false;
        _notEnoughMoneyPopup.OnClose -= CloseNotEnoughMoneyPopup;
        _uiManager.HidePopup();
    }

    private void SetTargetSlot()
    {
        var cameraXpos = _camera.transform.position.x;
        _targetSlot = Enumerable.Range(0, _slotsCount).OrderBy(x => Mathf.Abs(x * -_gridSize - cameraXpos))
            .FirstOrDefault();
        CheckForUnlockCar();
    }
    
    private void OnTouchBegan()
    {
        if(_isNotEnoughMoneyPopupActive) return;
        
        _snapEnabled = false;
        _isSwipeDetected = false;
    }

    private void OnTouchEnded()
    {
        if(_isNotEnoughMoneyPopupActive) return;
        
        _snapEnabled = true;
        SetTargetSlot();
    }


    private void OnSwipe(Vector2 vector)
    {
        if(_isNotEnoughMoneyPopupActive) return;
        
        var amount = vector.x;
        if (Mathf.Abs(amount) > _swipeDetectionThreshold)
            _isSwipeDetected = true;
        var position = _camera.transform.position;
        position += _pixelToUnitsRatio * amount * Vector3.right;
        // position = Vector3.MoveTowards(position, position + _pixelToUnitsRatio * amount * Vector3.right,
        //     Time.deltaTime * _cameraMoveSpeed);
        _camera.transform.position = position;
    }

    private void Update()
    {
        if (_snapEnabled == false) return;

        var targetPos = _camera.transform.position;
        targetPos.x = _targetSlot * -_gridSize;

        _camera.transform.position = Vector3.Lerp(_camera.transform.position, targetPos,
            Time.deltaTime * _cameraMoveSpeed);
    }
}