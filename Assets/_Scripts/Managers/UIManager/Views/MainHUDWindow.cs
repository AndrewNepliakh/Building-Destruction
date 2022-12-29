using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class MainHUDWindow : Window
{
    public event Action OnTapPlay;

    public GameObject Speedometr => _speedometr;
  
    [SerializeField] private Button _tapToPlayButton;
    [SerializeField] private Image _speedometrImage;
    [SerializeField] private GameObject _speedometr;
    [Space(20)] [SerializeField] private List<GameObject> _tryCars = new();

    [Inject] private SignalBus _signalBus;

    public override void Show(UIViewArguments arguments)
    {
        base.Show(arguments);
        _tapToPlayButton.gameObject.SetActive(true);
        _signalBus.Subscribe<GameRestartSignal>(Reset);
        _tapToPlayButton.onClick.AddListener(OnTapPlayHandler);
        _speedometrImage.fillAmount = 0.0f;
    }

    public void UpdateSpeedometr(float value)
    {
        _speedometrImage.fillAmount = value;
    }

    public void UpdateTryCars(int trys)
    {
        if(trys == 0) return;
        
        for (var i = 0; i < trys; i++)
        {
            _tryCars[i].SetActive(false);
        }
    }

    private void OnTapPlayHandler()
    {
        OnTapPlay?.Invoke();
    }

    public void HideTapPlayButton()
    {
        _tapToPlayButton.gameObject.SetActive(false);
    }

    private void ShowTapPlayButton()
    {
        _tapToPlayButton.gameObject.SetActive(true);
    }
    
    public override void Reset()
    {
        ShowTapPlayButton();
        _speedometr.SetActive(true);

        foreach (var tryCar in _tryCars)
        {
            tryCar.SetActive(true);
        }
    }

    public override void Hide(UIViewArguments arguments)
    {
        base.Hide(arguments);
        Reset();
        _signalBus.Unsubscribe<GameRestartSignal>(Reset);

        _tapToPlayButton.onClick.RemoveListener(OnTapPlayHandler);
    }
}