using System;
using UnityEngine;

public interface ICarControllerHandler
{
    void Enable();
    void Disable();
    float GetCurrentSpeed();
}

public class CarController : MonoBehaviour, ICarController, ICarControllerHandler
{
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _acceleration = 3f;
    [SerializeField] private float _deceleration = 2f;
    [SerializeField] private float _steerSpeed = 2f;
    [SerializeField] private float _steerClampValue;
    [SerializeField] private Transform _forceCenter;
    public float CurrentSpeed => _currentSpeed;
    public event Action<float> OnSpeedUpdate;
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
        _targetSteerSpeed = value;
    }

    private void FixedUpdate()
    {
        if (_enabled == false || _groundDetector.IsGrounded() == false) return;

        if (_isAcceleratorDepressed)
        {
            _rb.AddForceAtPosition(_acceleration * transform.forward, _forceCenter.position);
        }
        else
        {
            _rb.AddForce(-_deceleration * transform.forward);
            float velocityToForwardThreshold = .9f;
            float minVelocityMagnitude = .1f;
            if (Mathf.Abs(Vector3.Dot(_rb.velocity.normalized, transform.forward)) > velocityToForwardThreshold &&
                _rb.velocity.magnitude < minVelocityMagnitude)
            {
                _rb.velocity = Vector3.zero;
            }
        }

        _currentSpeed = _rb.velocity.magnitude;

        _currentSteerSpeed =
            Mathf.MoveTowards(_currentSteerSpeed, _targetSteerSpeed, Time.fixedDeltaTime * _steerSpeed);

        var currentPosition = _rb.position;
        currentPosition += _currentSteerSpeed * _steerSpeed * transform.right;

        currentPosition = currentPosition.ClampX(_steerClampValue);
        _rb.position = currentPosition;
    }

    public void Enable() => _enabled = true;

    public void Disable() => _enabled = false;

    public float GetCurrentSpeed() => _currentSpeed;
}