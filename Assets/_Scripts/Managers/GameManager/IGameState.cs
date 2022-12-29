public interface IGameState
{
    void StartGame(IGameSceneContext ctx);
    void FallFromPlatform(IGameSceneContext ctx);
    void TakeOff(IGameSceneContext ctx);

    void Land(IGameSceneContext ctx);

    void EndAttempt(IGameSceneContext ctx);
    void Next(IGameSceneContext ctx);
    void NextLevel(IGameSceneContext ctx);
    void EndGame(IGameSceneContext ctx);

    void OnStateEnter(IGameSceneContext ctx);
    void OnStateExit(IGameSceneContext ctx);
}