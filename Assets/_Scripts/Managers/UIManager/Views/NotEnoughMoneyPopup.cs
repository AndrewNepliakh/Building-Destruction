using System;
using Managers;
using TMPro;
using UnityEngine;

public class NotEnoughMoneyPopup : Popup
{
    public event Action OnClose;

    [SerializeField] private ButtonScalable _closeButton;
    [SerializeField] private TextMeshProUGUI _currencyText;

    public override void Show(UIViewArguments arguments)
    {
        base.Show(arguments);
        _currencyText.text = arguments.Difference.ToString();
        _closeButton.AddListener(OnCloseHandler);
    }

    private void OnCloseHandler()
    {
        OnClose?.Invoke();
    }

    public override void Hide(UIViewArguments arguments)
    {
        base.Hide(arguments);
        
        _closeButton.RemoveListener(OnCloseHandler);
    }
}