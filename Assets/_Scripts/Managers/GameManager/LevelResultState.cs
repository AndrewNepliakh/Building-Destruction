public class LevelResultState : GameStateBase
{
    public override void OnStateEnter(IGameSceneContext ctx)
    {
        var controller = (GameSceneController)ctx;
        controller.ShowEndGameWindow();
    }

    public override void NextLevel(IGameSceneContext ctx)
    {
        ctx.SetState(new BeforeGameStartedState());
    }
}