using System;
using System.Collections;
using Madkpv;
using Managers;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerResetController : MonoBehaviour, IPlayerResetController, IWaitForStart
{
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    Rigidbody _rb;
    private IGroundDetector _groundDetector;
    [SerializeField] float _stallDetectionInterval = .1f;
    [SerializeField] float _speedThreshold = 1;
    [SerializeField] float _groundLevel = 0;
    [Inject] private ILevelManager _levelManager;
    [Inject] private SignalBus _signalBus;

    private IResetable[] _resetables;
    private CollisionMeshDeform[] _meshDeformers;
    private DetachablePart[] _detachableParts;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _groundDetector = GetComponent<IGroundDetector>();
        _levelManager.OnLevelLoaded += OnLevelLoaded;

        _meshDeformers = GetComponentsInChildren<CollisionMeshDeform>();
        _resetables = GetComponentsInChildren<IResetable>();
        _detachableParts = GetComponentsInChildren<DetachablePart>();

        _signalBus.Subscribe<GameAttemptStartedSignal>(OnAttemptStarted);
        _signalBus.Subscribe<OnBeforeGameStartSignal>(OnBeforeStart);
    }

    private void OnAttemptStarted()
    {
        ResetPlayerPosition();
    }

    private void OnBeforeStart()
    {
        ResetPlayerToOriginalState();
    }

    private void OnLevelLoaded(ILevel level)
    {
        InitStartPosition(level.SpawnPoint);
        ResetPlayerPosition();
    }

    private void Start()
    {
        _startRotation = transform.rotation;
        InitStartPosition(transform.position);
    }

    private void InitStartPosition(Vector3 spawnPoint) => _startPosition = spawnPoint;

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<GameAttemptStartedSignal>(OnAttemptStarted);
        _signalBus.Unsubscribe<OnBeforeGameStartSignal>(OnBeforeStart);

        _levelManager.OnLevelLoaded -= OnLevelLoaded;
    }

    public void StartAttemptEndDetection()
    {
        StopAllCoroutines();
        StartCoroutine(DetectAttemptEnd());
    }

    private IEnumerator DetectAttemptEnd()
    {
        yield return DetectTookOff();

        yield return DetectAttemptEndCondition();
    }

    private IEnumerator DetectAttemptEndCondition()
    {
        while (true)
        {
            if (transform.position.y < _groundLevel)
            {
                _signalBus.Fire<OnCarFallOff>();
                _signalBus.Fire<GameAttemptEndedSignal>();
                yield break;
            }

            if (_rb.velocity.magnitude < _speedThreshold) break;
            yield return Helpers.CachedDelay(_stallDetectionInterval);
        }

        _signalBus.Fire<GameAttemptEndedSignal>();
    }

    private IEnumerator DetectTookOff()
    {
        while (true)
        {
            if (_groundDetector.IsGrounded() == false) break;
            yield return Helpers.CachedDelay(_stallDetectionInterval);
        }
    }

    public void OnStart()
    {
        StartAttemptEndDetection();
    }

    public void ResetPlayerToOriginalState()
    {
        StopAllCoroutines();
        StartCoroutine(FrameDistributedResetRoutine(true));
    }

    public void ResetPlayerPosition()
    {
        StopAllCoroutines();
        StartCoroutine(FrameDistributedResetRoutine());
    }
    
    IEnumerator FrameDistributedResetRoutine(bool resetToOriginalState = false)
    {
        MakeJointsUbreakable();
        yield return null;
        transform.rotation = _startRotation;
        transform.position = _startPosition;
        _rb.velocity = _rb.angularVelocity = Vector3.zero;
        yield return null;
        if (resetToOriginalState)
        {
            ResetResetables();
            ResetDeforms();
        }

        yield return null;
        MakeJointsBreakable();
        StartAttemptEndDetection();
    }

    private void ResetDeforms()
    {
        foreach (var meshDeformer in _meshDeformers)
            meshDeformer.ResetDeform();
    }

    private void ResetResetables()
    {
        foreach (var resetable in _resetables)
            resetable.OnResetPlayer();
    }

    private void MakeJointsUbreakable()
    {
        foreach (var detachablePart in _detachableParts)
        {
            detachablePart.MakeJointUnbreakable();
        }
    }

    private void MakeJointsBreakable()
    {
        foreach (var detachablePart in _detachableParts)
        {
            detachablePart.MakeJointBreakable();
        }
    }
}