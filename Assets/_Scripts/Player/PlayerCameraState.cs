using Cinemachine;
using Madkpv;

public class PlayerCameraState : IState
{
    private int _targetPriority = 100;
    private readonly CinemachineVirtualCamera _camera;
    private int _oldPriority;

    public PlayerCameraState(CinemachineVirtualCamera camera)
    {
        _camera = camera;
    }

    public void Tick() { }

    public void OnEnter()
    {
        _oldPriority = _camera.Priority;
        _camera.Priority = _targetPriority;
    }

    public void OnExit()
    {
        _camera.Priority = _oldPriority;
    }
}