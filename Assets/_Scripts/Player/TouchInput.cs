using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchInput : MonoBehaviour
{
    public event Action OnTouchBegan;
    public event Action OnTouchEnded;
    public event Action<float> OnHorizontalMove;
    public event Action<float> OnVerticalMove;
    public event Action<Vector2> OnMove;
    private bool _enabled = false;

    [SerializeField] private float _swipeDetectThreshold = .1f;
    [SerializeField] private float _swipeDirectionDetectThreshold = .7f;
    private Vector2 _previousPosition = Vector2.zero;
    private bool _isTouchBegan = false;

    private void Awake() => EnhancedTouchSupport.Enable();

    private IEnumerator Start()
    {
        yield return null;
        _enabled = true;
    }

    void Update()
    {
        if (_enabled == false) return;
        if (Touch.activeFingers.Count < 1) return;

        var touch = Touch.activeFingers[0].currentTouch;

        if (touch.phase == TouchPhase.Began)
        {
            OnTouchBegan?.Invoke();
            _previousPosition = touch.screenPosition;
            _isTouchBegan = true;
            return;
        }
        
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            OnTouchEnded?.Invoke();
            OnVerticalMove?.Invoke(0);
            OnHorizontalMove?.Invoke(0);
            OnMove?.Invoke(Vector2.zero);
            _isTouchBegan = false;
            return;
        }

        
        if (touch.phase == TouchPhase.Moved)
        {
            if (_isTouchBegan == false)
            {
                _previousPosition = touch.screenPosition;
                _isTouchBegan = true;
                OnTouchBegan?.Invoke();
            }
            var currentPosition = touch.screenPosition;
            var delta = currentPosition - _previousPosition;
            _previousPosition = currentPosition;
            
            if (delta.magnitude > _swipeDetectThreshold)
            {
                var dir = delta.normalized;
                OnMove?.Invoke(delta);
                if (Mathf.Abs(Vector2.Dot(Vector2.up, dir)) > _swipeDirectionDetectThreshold)  OnVerticalMove?.Invoke(delta.y);
                if (Mathf.Abs(Vector2.Dot(Vector2.right, dir)) > _swipeDirectionDetectThreshold) OnHorizontalMove?.Invoke(delta.x);
            }
            else
            {
                OnVerticalMove?.Invoke(0);
                OnHorizontalMove?.Invoke(0);
                OnMove?.Invoke(Vector2.zero);
            }
        } 
    }
}