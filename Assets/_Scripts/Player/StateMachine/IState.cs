namespace Madkpv
{
    public interface IState
    {
        void Tick();
        void OnEnter();
        void OnExit();
    }
}