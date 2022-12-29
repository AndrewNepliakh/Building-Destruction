using System;
using UnityEngine;

public abstract class GameSceneStateMachine : MonoBehaviour, IGameSceneContext
{
    public event Action<Type> OnStateEnter;
    public event Action<Type> OnStateExit;
    protected IGameState _state = new BeforeGameStartedState();

    void IGameSceneContext.SetState(IGameState newState)
    {
        OnStateExit?.Invoke(_state.GetType());
        _state.OnStateExit(this);
        _state = newState;
        _state.OnStateEnter(this);
        OnStateEnter?.Invoke(_state.GetType());
    }

    public virtual void StartGame() => _state.StartGame(this);
    public virtual void EndAttempt() => _state.EndAttempt(this);
    public virtual void EndGame() => _state.EndGame(this);
    public virtual void TakeOff() => _state.TakeOff(this);
    public virtual void FallFromPlatform() => _state.FallFromPlatform(this);
    public virtual void Next() => _state.Next(this);
    public virtual void NextLevel() => _state.NextLevel(this);
}