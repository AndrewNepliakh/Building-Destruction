using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ButtonScalable : Button, IButtonScalable
    {
        public TextMeshProUGUI Text => _text;
        public Image Image => _image;

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;

        private Action _callBack;
        private bool _isPressed;
        private float _delay = 0.2f;
        private float _deltaSize = -0.2f;
        private float _multiplierForMilliseconds = 0.001f;

        private Coroutine _coroutine;
        private float _amplitude = 0.3f;

        private IEnumerator OnPressButton()
        {
            _isPressed = false;

            while (!_isPressed)
            {
                yield return null;
            }

            var counter = _delay;

            while (counter > 0.0f)
            {
                var tan = Mathf.Tan(1.0f - Mathf.InverseLerp(_delay, 0.0f, counter) * 2.0f);
                var discreteness = 1.0f - (1.0f - Mathf.Abs(tan)) * _amplitude;
                transform.localScale = new Vector3(discreteness, discreteness, discreteness);
                counter -= Time.deltaTime;

                yield return null;
            }

            transform.localScale = Vector3.one;

            if (_callBack != null)
                _coroutine = _coroutine.Start(this, OnPressButton());

            _callBack?.Invoke();
        }

        public void AddListener(Action callBack)
        {
            _callBack += callBack;
            onClick.AddListener(OnClick);
            
            if (gameObject.activeSelf)
                _coroutine = _coroutine.Start(this, OnPressButton());
        }

        public void RemoveListener(Action callBack)
        {
            _callBack -= callBack;
            onClick.RemoveListener(OnClick);
        }

        public void Hide()
        {
            interactable = false;
            if (_image != null)
            {
                _image.color = Color.clear;
                _image.raycastTarget = false;
            }
        }

        public void Show()
        {
            interactable = true;
            if (_image != null)
            {
                _image.color = Color.white;
                _image.raycastTarget = true;
            }
        }

        private void OnClick() => _isPressed = true;
    }

    public interface IButtonScalable
    {
        void AddListener(Action callBack);
        void RemoveListener(Action callBack);
        void Hide();
        void Show();
    }
}