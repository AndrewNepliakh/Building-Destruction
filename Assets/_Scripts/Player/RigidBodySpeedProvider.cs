using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodySpeedProvider : MonoBehaviour, ISpeedProvider
{
    public float CurrentSpeed => _rb.velocity.magnitude;
    private Rigidbody _rb;
    private void Awake() => _rb = GetComponent<Rigidbody>();
}
