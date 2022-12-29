using System;

public interface ICarController
{
    void Deceleration();
    void Acceleration();
    void Steer(float value);
    float CurrentSpeed { get; }
    event Action<float> OnSpeedUpdate;
}