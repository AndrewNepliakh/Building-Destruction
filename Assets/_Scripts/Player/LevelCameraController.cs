using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class LevelCameraController : MonoBehaviour
{
    private GameSceneController _gameSceneController;
    [SerializeField] private int _levelCameraPriority = 200;
    [SerializeField] private GameObject _cameraParentGameObject;
    [SerializeField] CamSwitchTimeRange _cameraSwitchTimeRange;
    private Type _overviewCameraState = typeof(DestroyOverviewState);
    private Type _levelResultsCameraState = typeof(LevelResultState);
    private bool _enabled = false;
    private ICameraController[] _cameraControllers;
    ICameraController _currentCamera;


    private void Awake()
    {
        _gameSceneController = GetComponent<GameSceneController>();
        _gameSceneController.OnStateEnter += OnSceneStateEnter;
        _gameSceneController.OnStateExit += OnSceneStateExit;
    }

    public void Init(ICameraController[] cameraControllers)
    {
        _cameraControllers = cameraControllers;
    }

    IEnumerator OverviewCameraRoutine()
    {
        var random = new System.Random();
        while (_enabled)
        {
            var randomCams = _cameraControllers.OrderBy(x => random.Next()).ToList();

            foreach (var randomCam in randomCams)
            {
                if (randomCam == _currentCamera) continue;
                ResetLastCamera();
                _currentCamera = randomCam;
                randomCam.StartCamera();

                yield return new WaitForSeconds(_cameraSwitchTimeRange.Random);
            }
        }
    }

    private void ResetLastCamera()
    {
        if (_currentCamera != null)
            _currentCamera.StopCamera();
    }

    private void OnSceneStateExit(Type state)
    {
        if (state != _overviewCameraState && state != _levelResultsCameraState) return;
        _enabled = false;
        StopAllCoroutines();
        ResetLastCamera();
    }

    private void OnSceneStateEnter(Type state)
    {
        if (_enabled) return;
        if (state != _overviewCameraState && state != _levelResultsCameraState ) return;
        if (_cameraControllers.Length == 0) return;
        
        _enabled = true;
        StartCoroutine(OverviewCameraRoutine());
    }

    [Serializable]
    private struct CamSwitchTimeRange
    {
        public float Min;
        public float Max;
        public float Random => UnityEngine.Random.Range(Min, Max);
    }
}