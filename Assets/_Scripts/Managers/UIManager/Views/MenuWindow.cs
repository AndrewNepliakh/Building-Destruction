using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MenuWindow : Window
{
    public Action OnSettingsClicked;
    public Action OnVehicleClicked;
    public Action OnDailyClicked;
    public Action OnNoAdsClicked;

    public List<LevelUI> LevelsButton => _levelsButton;

    [Inject] private IUserManager _userManager;
    
    [SerializeField] private TextMeshProUGUI _currencyText;
    
    [SerializeField] private ButtonScalable _settingsButton;
    [SerializeField] private ButtonScalable _vehicleButton;
    [SerializeField] private ButtonScalable _dailyButton;
    [SerializeField] private ButtonScalable _noAdsButton;

    [SerializeField] private List<LevelUI> _levelsButton;
    [SerializeField] private List<Image> _dots;

    [SerializeField] private Color _unlockedColor;
    
    public override void Show(UIViewArguments arguments)
    {
        base.Show(arguments);

        _currencyText.text = "$ " + _userManager.CurrentUser.Currency;

        for (var i = 0; i < _userManager.CurrentUser.UserProgress.Levels.Length - 1; i++)
        {
            if (_userManager.CurrentUser.UserProgress.Levels[i].Stars > 0)
            {
                _dots[i].color = _unlockedColor;
            }
        }
        
        _settingsButton.AddListener(OnSettingsButtonClicked);
        _vehicleButton.AddListener(OnVehicleButtonClicked);
        _dailyButton.AddListener(OnDailyButtonClicked);
        _noAdsButton.AddListener(OnNoAdsButtonClicked);
    }

    private void OnSettingsButtonClicked()
    {
        OnSettingsClicked?.Invoke();
    }

    private void OnVehicleButtonClicked()
    {
        OnVehicleClicked?.Invoke();
    }

    private void OnDailyButtonClicked()
    {
        OnDailyClicked?.Invoke();
    }

    private void OnNoAdsButtonClicked()
    {
        OnNoAdsClicked?.Invoke();
    }


    public override void Reset()
    {
    }

    public override void Hide(UIViewArguments arguments)
    {
        base.Hide(arguments);
        
        _settingsButton.RemoveListener(OnSettingsButtonClicked);
        _vehicleButton.RemoveListener(OnVehicleButtonClicked);
        _dailyButton.RemoveListener(OnDailyButtonClicked);
        _noAdsButton.RemoveListener(OnNoAdsButtonClicked);
        
    }
}