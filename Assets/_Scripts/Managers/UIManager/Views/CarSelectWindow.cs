using System;
using Managers;
using TMPro;
using UnityEngine;
using Zenject;

public class CarSelectWindow : Window
{
    public GameObject Cross => _cross;
    public ButtonScalable PlayButton => _playButton;
    public ButtonScalable BuyButton => _buyButton;
    
    public Action OnPlayTapped;
    public Action OnBackToMenu;
    public Action OnBuyNowButtonClicked;

    [Inject] private GameSettingsSO _settings;
    [Inject] private IUserManager _userManager;

    [SerializeField] private ButtonScalable _playButton;
    [SerializeField] private ButtonScalable _buyButton;
    [SerializeField] private ButtonScalable _backToMenuButton;
    [Space(20)]
    [SerializeField] private TextMeshProUGUI _currencyText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [Space(20)]
    [SerializeField] private GameObject _cross;
    [SerializeField] private GameObject _price;

    public override void Show(UIViewArguments arguments)
    {
        base.Show(arguments);
        
        _currencyText.text = "$ " + _userManager.CurrentUser.Currency;

        _playButton.AddListener(OnTapPlayButton);
        _buyButton.AddListener(OnBuyNowButton);
        _backToMenuButton.AddListener(OnBackToMenuButton);
    }
    
    private void OnTapPlayButton()
    {
        OnPlayTapped?.Invoke();
    }

    private void OnBuyNowButton()
    {
        OnBuyNowButtonClicked?.Invoke();
        _currencyText.text = "$ " + _userManager.CurrentUser.Currency;
    }

    private void OnBackToMenuButton()
    {
        OnBackToMenu?.Invoke();
    }

    public override void Reset()
    {
    }

    public void UpdatePrice(int targetCar)
    {
        _price.SetActive(targetCar > 0);
        IGameSettingsEditor settings = _settings;
        _priceText.text = "$ " + settings.Cars.Cars[targetCar].Price;
    }

    public override void Hide(UIViewArguments arguments)
    {
        base.Hide(arguments);

        _playButton.RemoveListener(OnTapPlayButton);
        _buyButton.RemoveListener(OnBuyNowButton);
        _backToMenuButton.RemoveListener(OnBackToMenuButton);
    }
}