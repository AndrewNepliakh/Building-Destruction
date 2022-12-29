using UnityEngine;

public class CarSoundBridge: MonoBehaviour
{
    private ISpeedProvider _speedProvider;
    private CarSoundController _soundController;

    private void Awake()
    {
        _speedProvider = GetComponent<ISpeedProvider>();
        _soundController = GetComponent<CarSoundController>();
    }

    private void Update()
    {
        _soundController.UpdateSound(_speedProvider.CurrentSpeed);
    }
}