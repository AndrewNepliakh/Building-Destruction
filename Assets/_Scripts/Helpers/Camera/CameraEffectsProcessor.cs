using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CameraEffectsProcessor : MonoBehaviour
{
    private Vector3 _cameraOffset;
    private Transform _cameraTarget;
    private Vector3 _previousPosition;
    private Quaternion _previousRotation;
    private List<ICameraProcess> _processes;

    void Start()
    {
        _previousPosition = transform.position;
        _previousRotation = transform.rotation;
        _cameraOffset = transform.parent.position - transform.position;
        _cameraTarget = transform.parent;
        _processes = new List<ICameraProcess>(GetComponents<ICameraProcess>()).OrderBy(p => p.Priority).ToList();
    }

    private void LateUpdate()
    {
        var targetRotation = Quaternion.LookRotation(_cameraTarget.position - _previousPosition);
        var targetPosition = _cameraTarget.position -
                             Quaternion.FromToRotation(Vector3.forward, _cameraTarget.forward) * _cameraOffset;

        foreach (var cameraProcess in _processes)
        {
            (targetPosition, targetRotation) =
                cameraProcess.Process(_previousPosition, _previousRotation, targetPosition, targetRotation, _cameraTarget);
        }

        _previousPosition = transform.position = targetPosition;
        _previousRotation = transform.rotation = targetRotation;
    }
}