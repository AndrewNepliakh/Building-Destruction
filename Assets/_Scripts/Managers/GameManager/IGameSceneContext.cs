using UnityEngine;

public interface IGameSceneContext
{
    GameObject gameObject { get; }
    void SetState(IGameState newState);
}