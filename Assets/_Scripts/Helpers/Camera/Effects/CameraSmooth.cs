using UnityEngine;

public class CameraSmooth : MonoBehaviour, ICameraProcess
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _movementSpeed;
    [field: SerializeField] public int Priority { get; private set; }

    public (Vector3, Quaternion) Process(Vector3 previousPosition, Quaternion previousRotation, Vector3 targetPosition,
        Quaternion targetRotation, Transform targetTransform = null)
    {
        var processedRotation = Quaternion.Lerp(previousRotation, targetRotation, _rotationSpeed * Time.deltaTime);

        var processedPosition = Vector3.Lerp(previousPosition, targetPosition, Time.deltaTime * _movementSpeed);

        return (processedPosition, processedRotation);
    }
}