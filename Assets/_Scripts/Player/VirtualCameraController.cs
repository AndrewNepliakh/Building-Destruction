using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirtualCameraController : MonoBehaviour, ICameraController
{
    [SerializeField] private int _targetPriority = 200;
    private CinemachineVirtualCamera _virtualCamera;
    private int _oldPriority;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
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
