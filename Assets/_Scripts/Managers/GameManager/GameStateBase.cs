public abstract class GameStateBase : IGameState
{
    public virtual void StartGame(IGameSceneContext ctx) { }

    public virtual void FallFromPlatform(IGameSceneContext ctx) { }

    public virtual void TakeOff(IGameSceneContext ctx) { }

    public virtual void Land(IGameSceneContext ctx) { }

    public virtual void EndAttempt(IGameSceneContext ctx) { }

    public virtual void Next(IGameSceneContext ctx) { }
    
    public virtual void NextLevel(IGameSceneContext ctx) { }

    public virtual void EndGame(IGameSceneContext ctx) { }
    
    public virtual void OnStateEnter(IGameSceneContext ctx) { }

    public virtual void OnStateExit(IGameSceneContext ctx) { }
}