using UnityEngine;
using Zenject;

public class CameraSineShaker : MonoBehaviour, ICameraProcess
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] private AnimationCurve _shakeAmplitudeProfile;
    [SerializeField] private float _shakeAmplitude = 1;
    [SerializeField] private float _shakeFrequency = 60;
    [SerializeField] private float _shakeTime = 1;
    [SerializeField] private float _maxAffectedDistance = 200;
    [SerializeField] private float _minAffectedDistance = 50;

    private float _baseShakeAmount;
    private bool _shakeEnabled = false;
    private float _currentTime = 0;

    [field: SerializeField] public int Priority { get; private set; }

    private void Awake() => _signalBus.Subscribe<ExplosionOccuredSignal>(ExplosionFeedback);

    private void OnDestroy() => _signalBus.Unsubscribe<ExplosionOccuredSignal>(ExplosionFeedback);

    private void ExplosionFeedback(ExplosionOccuredSignal eventData)
    {
        var distanceToExplosion = (eventData.Position - transform.position).magnitude - _minAffectedDistance;
        distanceToExplosion = distanceToExplosion < 0 ? 0 : distanceToExplosion;

        _baseShakeAmount = 1 - distanceToExplosion / _maxAffectedDistance;
        if (_baseShakeAmount > .1f)
        {
            _currentTime = 0;
            _shakeEnabled = true;
        }
    }

    public (Vector3, Quaternion) Process(Vector3 previousPosition, Quaternion previousRotation, Vector3 targetPosition,
        Quaternion targetRotation, Transform targetTransform = null)
    {
        var shakeAmount = Vector3.zero;
        if (_shakeEnabled)
            shakeAmount = ShakeFunction();

        return (targetPosition + shakeAmount, targetRotation);
    }

    private Vector3 ShakeFunction()
    {
        float xValue = Mathf.Sin(Time.time * _shakeFrequency) *
                       _shakeAmplitudeProfile.Evaluate(_currentTime / _shakeTime) * _baseShakeAmount * _shakeAmplitude;
        _currentTime += Time.deltaTime;

        if (_currentTime > _shakeTime) _shakeEnabled = false;

        return Vector3.right * xValue;
    }
}