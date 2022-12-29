using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class OrbitCameraController : MonoBehaviour, ICameraController
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineOrbitalTransposer _transposer;
    [SerializeField] private float _speed = 15;
    [SerializeField] private int _targetPriority = 200;
    private int _oldPriority;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    private void Update()
    {
        _transposer.m_XAxis.Value += _speed * Time.deltaTime;
    }

    public void StartCamera()
    {
        _oldPriority = _virtualCamera.Priority;
        _virtualCamera.Priority = _targetPriority;

    }

    public void StopCamera()
    {
        _virtualCamera.Priority = _oldPriority;
    }
}
