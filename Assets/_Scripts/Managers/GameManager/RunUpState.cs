public class RunUpState : GameStateBase
{
    public override void OnStateEnter(IGameSceneContext ctx) => ((GameSceneController)ctx).StartAttempt();

    public override void TakeOff(IGameSceneContext ctx) => ctx.SetState(new DestroyState());
}