using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DollyCameraController : MonoBehaviour, ICameraController
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineTrackedDolly _transposer;
    [SerializeField] private float _speed = .1f;
    [SerializeField] private int _targetPriority = 200;
    private bool _enabled = false;
    private CinemachineBlendDefinition _blendDeletage;
    private int _oldPriority;


    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        
    }

    public void StartCamera()
    {
        _enabled = true;
        _oldPriority = _virtualCamera.Priority;
        _virtualCamera.Priority = _targetPriority;
    }

    public void StopCamera()
    {
        _transposer.m_PathPosition = 0;
        _enabled = false;
        _virtualCamera.Priority = _oldPriority;
    }

    private void Update()
    {
        if (_enabled == false) return;
        _transposer.m_PathPosition  = Mathf.Lerp(_transposer.m_PathPosition, 1, _speed * Time.deltaTime);
    }
}
