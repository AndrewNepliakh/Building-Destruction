using System.Collections;
using UnityEngine;

public class DestroyState : GameStateBase
{
    public override void EndAttempt(IGameSceneContext ctx)
    {
        ctx.SetState(new DestroyOverviewState());
        // ((MonoBehaviour)ctx).StartCoroutine(AwaitLevelResultAtNextFrame(ctx));
    }

    IEnumerator AwaitLevelResultAtNextFrame(IGameSceneContext ctx)
    {
        yield return null;
        if (((GameSceneController)ctx).IsEnd)
        {
            ctx.SetState(new LevelResultState());
        }
        else
        {
            ctx.SetState(new DestroyOverviewState());
        }
    }

    public override void FallFromPlatform(IGameSceneContext ctx) => ctx.SetState(new DestroyOverviewState());

    public override void Land(IGameSceneContext ctx) => ctx.SetState(new RunUpState());

    public override void OnStateExit(IGameSceneContext ctx)
    {
        ((MonoBehaviour)ctx).StopAllCoroutines();
    }
}