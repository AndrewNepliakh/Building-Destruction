using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlowDeviceDetector : MonoBehaviour
{
    private float _thresholdFrameRate = 15;
    private bool _isDeviceFast;
    private float _testTimeWindow = 2;
    private float _probeLength = .5f;
    private float _timer;
    private float _timePassed;
    private int _frames;
    private float _frameRate;   
    private float _avgTimePerFrame;

    private void Start()
    {
        _isDeviceFast = PlayerPrefs.GetInt(Constants.FAST_DEVICE_PLAYERPREFS_KEY, 1) > 0;
    }

    private void Update()
    {
        if (_isDeviceFast == false) return;
        
        if (CheckAvgFrameRate() == false || Application.isFocused == false) return;
        
        if (_frameRate > _thresholdFrameRate) return;
        
        PlayerPrefs.SetInt(Constants.FAST_DEVICE_PLAYERPREFS_KEY, 0);
        _isDeviceFast = false;
    }

    bool CheckAvgFrameRate()
    {
        _timer -= Time.deltaTime;
        _timePassed += Time.deltaTime;
        _frames++;

        if (_timer <= 0)
        {
            _frameRate = _frames / _timePassed;
            _avgTimePerFrame = 1f / _frames;
            _frames = 0;
            _timePassed = 0;
            _timer = _probeLength;
            return true;
        }

        return false;
    }
}