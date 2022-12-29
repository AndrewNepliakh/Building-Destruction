using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = .1f;

    private void Update() => transform.rotation *= Quaternion.Euler(Time.deltaTime * _rotationSpeed * Vector3.up);
}