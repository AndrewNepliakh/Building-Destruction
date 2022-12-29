using System.Collections;
using UnityEngine;

public class DestroyOverviewState : GameStateBase
{
    IGameState _nextState;
    IEnumerator AwaitTimeOut(IGameSceneContext ctx)
    {
        yield return new WaitForSeconds(Constants.LEVEL_OVERVIEW_TIME);
        Next(ctx);
    }

    private IGameState GetNextState(IGameSceneContext ctx) => ((GameSceneController)ctx).IsEnd ? new LevelResultState() : new RunUpState();

    public override void Next(IGameSceneContext ctx)
    {
        ctx.SetState(GetNextState(ctx));
    }

    public override void OnStateEnter(IGameSceneContext ctx) => ((MonoBehaviour)ctx).StartCoroutine(AwaitTimeOut(ctx));

    public override void OnStateExit(IGameSceneContext ctx) => ((MonoBehaviour)ctx).StopAllCoroutines();
}