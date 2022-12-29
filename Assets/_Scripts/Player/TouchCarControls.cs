using System;
using UnityEngine;

[RequireComponent(typeof(TouchInput)), RequireComponent(typeof(ICarController))]
public class TouchCarControls : MonoBehaviour
{
    private TouchInput _touchInput;
    private ICarController _carController;

    private void Awake()
    {
        _carController = GetComponent<ICarController>();
        _touchInput = GetComponent<TouchInput>();
        _touchInput.OnHorizontalMove += _carController.Steer;
        _touchInput.OnTouchBegan += _carController.Acceleration;
        _touchInput.OnTouchEnded += _carController.Deceleration;
    }

    private void OnDestroy()
    {
        _touchInput.OnHorizontalMove -= _carController.Steer;
        _touchInput.OnTouchBegan -= _carController.Acceleration;
        _touchInput.OnTouchEnded -= _carController.Deceleration;
    }
}