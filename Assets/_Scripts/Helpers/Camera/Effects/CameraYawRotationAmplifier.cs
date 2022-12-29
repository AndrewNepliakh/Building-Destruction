using UnityEngine;

public class CameraYawRotationAmplifier : MonoBehaviour, ICameraProcess
{
    [SerializeField] private float _rotationAmount = 1;
    [field: SerializeField] public int Priority { get; private set; }

    private float _previousAngle = 0;
    [SerializeField] private float _speed = 1;
    private float _currentRotationSpeed;


    public (Vector3, Quaternion) Process(Vector3 previousPosition, Quaternion previousRotation, Vector3 targetPosition,
        Quaternion targetRotation, Transform targetTransform = null)
    {
        if (targetTransform == null) return (targetPosition, targetRotation);
        var angle = Vector3.SignedAngle(Vector3.forward, targetTransform.forward, Vector3.up);
        var rotationSpeed = (angle - _previousAngle) / Time.deltaTime;
        _previousAngle = angle;
        _currentRotationSpeed = Mathf.MoveTowards(_currentRotationSpeed, rotationSpeed, Time.deltaTime * _speed);
        var rotation = Quaternion.Euler(0, _rotationAmount * _currentRotationSpeed, 0);
        var dir = rotation * ( targetPosition - targetTransform.position);
        var processedPosition = targetTransform.position + dir;
        
        return (processedPosition, targetRotation);
    }
}