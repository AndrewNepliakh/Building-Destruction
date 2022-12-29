using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Madkpv;
using Zenject;

public class TakeOffDetector : MonoBehaviour, IWaitForStart
{
    public event Action<bool> OnGroundedChange; 
    private bool _enabled = true;
    [SerializeField] private float _pollDelay = .3f;
    private IGroundDetector _groundDetector;
    private bool _isGrounded;
    [Inject] private SignalBus _signalBus; 

    private void Awake()
    {
        _groundDetector = GetComponent<IGroundDetector>();
    }
    

    IEnumerator TakeOffDetect()
    {
        while(_enabled)
        {
            var isGrounded = _groundDetector.IsGrounded();
            if (isGrounded != _isGrounded)
            {
                OnGroundedChange?.Invoke(isGrounded);
                if (isGrounded == false) _signalBus.Fire<OnCarTookOff>();
            }
            _isGrounded = isGrounded;
            yield return Helpers.CachedDelay(_pollDelay);
        }
    }

    public void OnStart()
    {
        StopAllCoroutines();
        StartCoroutine(TakeOffDetect());
    }
}

public class OnCarTookOff { }
