using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    private ISpeedProvider _speedProvider;
    private float _wheelRadius = 1;

    private void Awake()
    {
        _speedProvider = GetComponentInParent<ISpeedProvider>();
        var mesh = GetComponent<MeshFilter>();
        if (mesh == null) return;
        var size = mesh.sharedMesh.bounds.size;
        _wheelRadius = size.y;
    }

    private void Update()
    {
        if (_speedProvider.CurrentSpeed < .1f) return;
        var speed = _speedProvider.CurrentSpeed;
        var angularSpeed = (speed / _wheelRadius);
        transform.Rotate(Vector3.right, angularSpeed);
    }
}