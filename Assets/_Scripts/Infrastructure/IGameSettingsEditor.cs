public interface IGameSettingsEditor
{
    void SelectCar(int carIndex);
    CarsSetSO Cars { get; }
}