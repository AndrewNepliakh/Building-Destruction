using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerActivationController : MonoBehaviour
{
    [Inject] SignalBus _signalBus;
    Rigidbody _rb;
    IWaitForStart[] _startAwaiters;
    private ICarControllerHandler _carController;

    private void Awake()
    {
        _carController = GetComponent<ICarControllerHandler>();
        _rb = GetComponent<Rigidbody>();
        _startAwaiters = GetComponentsInChildren<IWaitForStart>();

        _signalBus.Subscribe<GameAttemptEndedSignal>(OnAttemptEnd);
        _signalBus.Subscribe<GameStartedSignal>(OnGameStart);
        _signalBus.Subscribe<GameEndSignal>(OnGameEnd);
        _signalBus.Subscribe<OnBeforeGameStartSignal>(OnBeforeStart);

        _rb.isKinematic = true;
    }

    private void OnBeforeStart()
    {

    }

    private void OnAttemptEnd()
    {
    }

    private void OnGameEnd()
    {
        _rb.isKinematic = true;
        _carController.Disable();
 
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<GameStartedSignal>(OnGameStart);
        _signalBus.Unsubscribe<GameAttemptEndedSignal>(OnAttemptEnd);
        _signalBus.Unsubscribe<GameEndSignal>(OnGameEnd);
    }

    private void OnGameStart()
    {
        _rb.isKinematic = false;
        // _carController.Enable();
        foreach (var startAwaiter in _startAwaiters) startAwaiter.OnStart();
    }
}