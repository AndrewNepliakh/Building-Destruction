using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using Zenject;

public class CommonPopup : Popup
{
    public event Action OnNextLevel;
    public event Action OnMenuTapped;
    public event Action OnRestartTapped;

    [Inject] private IUIManager _uiManager;
    [Inject] private SignalBus _signalBus;

    [SerializeField] private ButtonScalable _nextButton;
    [SerializeField] private ButtonScalable _menuButton;
    [SerializeField] private ButtonScalable _restartButton;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI _nextButtonText;
    [SerializeField] private TextMeshProUGUI _popupText;
    [Space(10)]
    [SerializeField] private List<Star> _stars;

    public override void Show(UIViewArguments arguments)
    {
        base.Show(arguments);

        _nextButton.gameObject.SetActive(false);
        _menuButton.gameObject.SetActive(false);

        if (arguments.Stars > 0)
        {
            ProceedResult(arguments.Stars);
            _popupText.text = "You win!";
            _nextButtonText.text = "Next";
            
            _nextButton.gameObject.SetActive(true);
            _menuButton.gameObject.SetActive(true);

            _nextButton.AddListener(OnNextLevelHandler);
            _menuButton.AddListener(OnMenuTappedHandler);
            _restartButton.AddListener(OnRestartTappedHandler);
            
            _signalBus.Fire<OnLevelComplete>();
        }
        else if (arguments.Stars == 0)
        {
            _restartButton.gameObject.SetActive(false);
            _popupText.text = "You lose!";
            _nextButtonText.text = "Restart";
            
            _nextButton.gameObject.SetActive(true);
            _menuButton.gameObject.SetActive(true);

            _nextButton.AddListener(OnRestartTappedHandler);
            _menuButton.AddListener(OnMenuTappedHandler);
            
            _signalBus.Fire<OnLevelFailed>();
        }
    }

    private void OnMenuTappedHandler()
    {
        OnMenuTapped?.Invoke();
        HidePopup();
    }

    private void OnNextLevelHandler()
    {
        OnNextLevel?.Invoke();
        HidePopup();
    }

    private void OnRestartTappedHandler()
    {
        OnRestartTapped?.Invoke();
        HidePopup();
    }

    private void HidePopup()
    {
        _uiManager.HidePopup();
        ResetStars();
        _restartButton.gameObject.SetActive(true);
    }

    private void ResetStars()
    {
        foreach (var star in _stars)
        {
            star.Deactivate();
        }
    }

    private  void ProceedResult(int stars)
    {
        for (var i = 0; i <= stars - 1; i++)
        {
             _stars[i].Activate();
        }
    }

    public override void Hide(UIViewArguments arguments)
    {
        _nextButton.RemoveListener(OnNextLevelHandler);
        _nextButton.RemoveListener(OnRestartTappedHandler);
        _menuButton.RemoveListener(OnMenuTappedHandler);
        _restartButton.RemoveListener(OnRestartTappedHandler);
        base.Hide(arguments);
    }
}