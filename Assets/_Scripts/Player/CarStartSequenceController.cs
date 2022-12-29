using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarStartSequenceController : MonoBehaviour
{
    private Animator _animator;
    private ICarControllerHandler _carController;
    private TouchInput _touchInput;
    [SerializeField] private float _minSpeed;
    [SerializeField] private bool _playStartSequence = false;
    private int _idleHash = Animator.StringToHash("Idle");
    private int _startSequenceHash = Animator.StringToHash("CarStartSequence");
    private bool _isStarted = false;


    private void Awake()
    {
        _touchInput = GetComponent<TouchInput>();
        _animator = GetComponent<Animator>();
        _carController = GetComponent<ICarControllerHandler>();
        _touchInput.OnTouchBegan += OnStart;
        _touchInput.OnTouchEnded += OnStartCancel;
        _carController.Disable();
    }

    private void OnStartCancel()
    {
        StopAllCoroutines();
        StartCoroutine(LowSpeedDetect());
    }

    public void StartSequenceEnded()
    {
        _isStarted = true;
        _carController.Enable();
    }

    private void OnStart()
    {
        if (_playStartSequence)
        {
            StopAllCoroutines();
            if (_isStarted == false)
                _animator.Play(_startSequenceHash);
        } else {
            StartSequenceEnded();
        }
    }

    IEnumerator LowSpeedDetect()
    {
        if (_isStarted) yield break;

        while (_carController.GetCurrentSpeed() > _minSpeed)
        {
            yield return new WaitForSeconds(.1f);
        }

        _carController.Disable();
        _animator.Play(_idleHash);
    }
}