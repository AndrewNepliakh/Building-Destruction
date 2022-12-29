using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Madkpv;
using Managers;
using UnityEngine;
using Zenject;

public class PlayerCameraController : MonoBehaviour, IWaitForStart
{
    [SerializeField] private CinemachineVirtualCamera _backCamera;
    [SerializeField] private List<CinemachineVirtualCamera> _sideCamerasSet;
    [SerializeField, Header("Overview Cameras")] private CinemachineVirtualCamera _leftOverviewCam;
    [SerializeField] private CinemachineVirtualCamera _rightOverviewCam;
    [SerializeField] private float _minSpeedForOverview = 1;
    [Inject] private ILevelManager _levelManager;
    private int _previousPriority;
    private TakeOffDetector _takeOffDetector;
    private bool _isActive = false;
    private StateMachine _sm;
    private ISpeedProvider _speedProvider;
    private bool _isGrounded = true;

    private void Awake()
    {
        _sm = new StateMachine();
        _speedProvider = GetComponent<ISpeedProvider>();
        _takeOffDetector = GetComponent<TakeOffDetector>();
        _takeOffDetector.OnGroundedChange += OnGroundedChanged;

        var backwardCamera = new PlayerCameraState(_backCamera);
        var sideCamera = new RandomCameraState(_sideCamerasSet);
        var leftOverviewCamera = new PlayerCameraState(_leftOverviewCam);
        var rigthOverviewCamera = new PlayerCameraState(_rightOverviewCam);


        At(backwardCamera, sideCamera, () => !OnGrounded());
        At(sideCamera, backwardCamera , OnGrounded);
        // At(sideCamera,leftOverviewCamera,() =>  CarLeftSide() && OverviewSpeedRiched());
        // At(sideCamera, rigthOverviewCamera, () => !CarLeftSide() && OverviewSpeedRiched());
        Aat(backwardCamera,OnGrounded);

        _sm.SetState(backwardCamera);

        bool OnGrounded() => _isGrounded;
        bool CarLeftSide() => transform.position.x < 0;
        bool OverviewSpeedRiched() => _speedProvider.CurrentSpeed < _minSpeedForOverview;

        void At(IState from, IState to, Func<bool> condition) => _sm.AddTransition(from, to, condition);
        void Aat(IState to, Func<bool> condition) => _sm.AddAnyTransition(to, condition);
    }

    private void OnGroundedChanged(bool isGrounded)
    {
        _isGrounded = isGrounded;
    }

    private void Update() => _sm.Tick();

    public void OnStart()
    {
        
    }

    private void OnDestroy()
    {
        _takeOffDetector.OnGroundedChange -= OnGroundedChanged;
    }
}