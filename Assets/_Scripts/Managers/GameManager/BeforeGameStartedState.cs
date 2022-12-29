public class BeforeGameStartedState : GameStateBase
{
    public override void OnStateEnter(IGameSceneContext ctx)
    {
        ((GameSceneController)ctx).OnBeforeStart();
    }

    public override void StartGame(IGameSceneContext ctx) => ctx.SetState(new RunUpState());

    public override void OnStateExit(IGameSceneContext ctx) => ((GameSceneController)ctx).OnGameStart();
}