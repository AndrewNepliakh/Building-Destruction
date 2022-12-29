using System;
using Managers;
using Zenject;

public class GlobalMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        # region Signals
        
        Container.DeclareSignal<GameStartedSignal>();
        Container.DeclareSignal<GameRestartSignal>();
        Container.DeclareSignal<GameCrushedSignal>();
        Container.DeclareSignal<GameEndSignal>();
        Container.DeclareSignal<OnAfterCarJumpSignal>();
        Container.DeclareSignal<GameAttemptEndedSignal>();
        Container.DeclareSignal<GameAttemptStartedSignal>();
        Container.DeclareSignal<OnCarTookOff>();
        Container.DeclareSignal<OnCarFallOff>();
        Container.DeclareSignal<OnBeforeGameStartSignal>();
        Container.DeclareSignal<OnLevelComplete>();
        Container.DeclareSignal<OnLevelFailed>();
        
        # endregion

        Container.Bind<IGameManager>().To<GameManager>().AsSingle().NonLazy();
        Container.Bind<ISaveManager>().To<SaveManager>().AsSingle().NonLazy();
        Container.Bind<IUserManager>().To<UserManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IUIManager),typeof(IInitializable)).To<UIManager>().AsSingle().NonLazy();
        Container.Bind<ILevelManager>().To<LevelManager>().AsSingle().NonLazy();
        Container.Bind(typeof(ISoundManager),typeof(IInitializable)).To<SoundManager>().AsSingle().NonLazy();
        Container.Bind<IBuildingManager>().To<BuildingManager>().AsSingle().NonLazy();
        Container.Bind<IFacebookManager>().To<FacebookSDKManager>().AsSingle().NonLazy();
        
        Container.BindSignal<GameStartedSignal>().ToMethod<IStartGameHandler>(x => x.StartGame).From(x => x.FromComponentsInHierarchy().AsSingle());
        
    }
}