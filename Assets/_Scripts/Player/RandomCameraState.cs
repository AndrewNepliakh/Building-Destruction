using System.Collections.Generic;
using Cinemachine;
using Madkpv;
using UnityEngine;

public class RandomCameraState : IState
{
    private readonly List<CinemachineVirtualCamera> _cameras;
    private CinemachineVirtualCamera _currentCamera;
    private int _targetPriority = 100;
    private int _oldPriority;

    public RandomCameraState(List<CinemachineVirtualCamera> cameras)
    {
        _cameras = cameras;
    }

    public void Tick()
    {
    }

    public void OnEnter()
    {
        if (_cameras.Count < 1) return;
        _currentCamera = _cameras[Random.Range(0, _cameras.Count)];
        _oldPriority = _currentCamera.Priority;
        _currentCamera.Priority = _targetPriority;
    }

    public void OnExit()
    {
        if (_currentCamera == null) return;
        _currentCamera.Priority = _oldPriority;
    }
}