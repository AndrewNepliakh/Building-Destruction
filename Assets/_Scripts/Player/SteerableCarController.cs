using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SteerableCarController : MonoBehaviour, ICarController, ICarControllerHandler,ISpeedProvider
{
    public event Action<float> OnSpeedUpdate;

    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _acceleration = 3f;
    [SerializeField] private float _deceleration = 2f;
    [SerializeField] private float _steerSpeed = 2f;
    [SerializeField] private float _steerClampValue;
    [SerializeField] private Transform _forceCenter;
    [SerializeField] private float _maxRotationAngle = 15;
    [SerializeField] private Vector3 _gravity = Vector3.down * 9.8f;
    public float CurrentSpeed => _currentSpeed;
    private Rigidbody _rb;
    private float _currentSpeed = 0;
    private float _targetSpeed = 0;
    private bool _isAcceleratorDepressed = false;
    private float _targetSteerSpeed = 0;
    private float _currentSteerSpeed = 0;
    private bool _enabled = true;
    private bool _isGrounded = false;
    private IGroundDetector _groundDetector;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _groundDetector = GetComponent<IGroundDetector>();
        _rb.centerOfMass = _forceCenter.position;
    }

    public void Deceleration()
    {
        _isAcceleratorDepressed = false;
    }

    public void Acceleration()
    {
        _isAcceleratorDepressed = true;
    }

    public void Steer(float value)
    {
        value *= (float)Screen.width / 540 / 100;
        _targetSteerSpeed += value;
    }

    private void FixedUpdate()
    {
        if (_enabled == false || _groundDetector.IsGrounded() == false)
        {
            _currentSpeed = 0;
            return;
        }

        var velocityMagnitude = _rb.velocity.magnitude;
        _currentSpeed = velocityMagnitude;

        OnSpeedUpdate?.Invoke(velocityMagnitude / _maxSpeed);

        float targetSpeed;
        float acceleration = 0;
        Vector3 forceVector = Vector3.forward;
        float minimumSpeedThreshold = .1f;

        if (_isAcceleratorDepressed && velocityMagnitude < _maxSpeed)
        {
            targetSpeed = _maxSpeed;
            acceleration = _acceleration;
        }
        else if (_currentSpeed > minimumSpeedThreshold  && IsCarMovingForward())
        {
            forceVector *= -1;
            targetSpeed = 0;
            acceleration = _deceleration;
        }

        
        // _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, Time.fixedDeltaTime * acceleration);
        
        var normalizedSpeed = _currentSpeed / _maxSpeed;
        
        var clampRotationAngle = _maxRotationAngle * normalizedSpeed;
        
        _targetSteerSpeed = Mathf.Clamp(_targetSteerSpeed, -clampRotationAngle, clampRotationAngle);
        _currentSteerSpeed =
            Mathf.Lerp(_currentSteerSpeed, _targetSteerSpeed, Time.fixedDeltaTime * _steerSpeed);

        
        if (Mathf.Abs(_currentSteerSpeed) > .1f)
        {
            var currentRotation = transform.rotation.eulerAngles;
            _rb.rotation = Quaternion.Euler(currentRotation.x, _currentSteerSpeed, currentRotation.z);
        }
        
        _rb.AddForce(Time.fixedDeltaTime * acceleration * forceVector ,ForceMode.VelocityChange);
    }

    private bool IsCarMovingForward()
    {
        float _dirDetectThreshold = .8f;
        return Vector3.Dot(transform.forward, _rb.velocity) > _dirDetectThreshold;
    }

    public void Enable() => _enabled = true;

    public void Disable() => _enabled = false;

    public float GetCurrentSpeed() => _currentSpeed;
}