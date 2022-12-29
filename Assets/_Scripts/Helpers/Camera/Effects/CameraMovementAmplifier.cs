using System;
using UnityEngine;

public class CameraMovementAmplifier : MonoBehaviour, ICameraProcess
{
    [field: SerializeField] public int Priority { get; private set; }
    [SerializeField] private float _amplifyFactor = 1.5f;

    public (Vector3, Quaternion) Process(Vector3 previousPosition, Quaternion previousRotation, Vector3 targetPosition,
        Quaternion targetRotation, Transform targetTransform = null)
    {
        var processedPosition = previousPosition + (targetPosition - previousPosition) * _amplifyFactor;

        return (processedPosition, targetRotation);
    }
}