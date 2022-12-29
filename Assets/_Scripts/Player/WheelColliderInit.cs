using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class WheelColliderInit : MonoBehaviour
{
    private WheelCollider _wheelCollider;
    private float _startDamper;
    private const float SafeDamper = 100;
    private TakeOffDetector _takeOffDetector;
    private JointSpring _spring;

    private void Awake()
    {
        _takeOffDetector = GetComponentInParent<TakeOffDetector>();
        _wheelCollider = GetComponent<WheelCollider>();
        _takeOffDetector.OnGroundedChange += OnCarTookOff;
        _spring = _wheelCollider.suspensionSpring;
        _startDamper = _spring.damper;
    }

    private void OnCarTookOff(bool isGrounded)
    {
        _spring.damper = isGrounded ? _startDamper : SafeDamper;
        _wheelCollider.suspensionSpring = _spring;
    }

    private void Start()
    {
        _wheelCollider.motorTorque = 0.0001f;
    }
}