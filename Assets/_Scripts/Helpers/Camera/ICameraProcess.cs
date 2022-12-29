using UnityEngine;

public interface ICameraProcess
{
    int Priority { get; }

    (Vector3, Quaternion) Process(Vector3 previousPosition, Quaternion previousRotation, Vector3 targetPosition,
        Quaternion targetRotation, Transform targetTransform = null);
}