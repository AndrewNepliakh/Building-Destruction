using System;
using Managers;
using TMPro;
using UnityEngine;
using Zenject;

public class BuyCarWindow : Window
{
    public Action OnBackButtonClicked;
    public Action OnBuyNowButtonClicked;
    
    [Inject] private IUserManager _userManager;
    [Inject] private IUIManager _uiManager;
    [Inject] private GameSettingsSO _settings;

    [SerializeField] private ButtonScalable _backButton;
    [SerializeField] private ButtonScalable _buyNowButton;
    
    [SerializeField] private TextMeshProUGUI _currencyText; 
    [SerializeField] private TextMeshProUGUI _priceText;

    private int _targetCar;

    public override void Show(UIViewArguments arguments)
    {
        base.Show(arguments);
        _currencyText.text = "$ " + _userManager.CurrentUser.Currency;
        IGameSettingsEditor settings = _settings;
        _targetCar = arguments.TargetCar;
        _priceText.text = "$ " + settings.Cars.Cars[_targetCar].Price;
        
        _backButton.AddListener(OnBackButtonClickedHandler);
        _buyNowButton.AddListener(OnBuyNowButtonClickedHandler);
    }

    private void OnBackButtonClickedHandler()
    {
        OnBackButtonClicked?.Invoke();
    }

    private void OnBuyNowButtonClickedHandler()
    {
        OnBuyNowButtonClicked?.Invoke();
    }

    public override void Reset()
    {
    }

    public override void Hide(UIViewArguments arguments)
    {
        base.Hide(arguments);
        
        _backButton.RemoveListener(OnBackButtonClickedHandler);
        _buyNowButton.RemoveListener(OnBuyNowButtonClickedHandler);
    }
}