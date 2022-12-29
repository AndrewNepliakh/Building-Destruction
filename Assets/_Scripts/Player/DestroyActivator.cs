using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class DestroyActivator : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _activationSpeedThreshold = 10;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ISwitchablePart>(out var switchablePart) == false || _rb.velocity.magnitude < _activationSpeedThreshold) return;
        switchablePart.SwitchOff();
    }
}
