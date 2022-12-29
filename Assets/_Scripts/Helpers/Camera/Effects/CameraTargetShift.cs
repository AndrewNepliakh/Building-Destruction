using UnityEngine;

public class CameraTargetShift : MonoBehaviour, ICameraProcess
{
    [SerializeField] private Vector3 _angleCorrection;
    [field: SerializeField] public int Priority { get; private set; }

    public (Vector3, Quaternion) Process(Vector3 previousPosition, Quaternion previousRotation, Vector3 targetPosition,
        Quaternion targetRotation, Transform targetTransform = null)
    {
        var processedRotation = targetRotation * Quaternion.Euler(_angleCorrection);
        return (targetPosition, processedRotation);
    }
}