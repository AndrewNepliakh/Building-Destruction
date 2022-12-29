using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Zenject;

public class CameraBlendModeSwitcher : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    private CinemachineBrain _cinemachineBrain;

    private void Awake()
    {
        _cinemachineBrain = GetComponentInChildren<CinemachineBrain>();
        _signalBus.Subscribe<OnCarFallOff>(OnAttemptEnded);
        _signalBus.Subscribe<GameAttemptStartedSignal>(OnAttemptStarted);
    }

    private void OnAttemptStarted() => _cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 2);

    private void OnAttemptEnded() => _cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 2);

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<OnCarFallOff>(OnAttemptEnded);
        _signalBus.Unsubscribe<GameAttemptStartedSignal>(OnAttemptStarted);
    }
}