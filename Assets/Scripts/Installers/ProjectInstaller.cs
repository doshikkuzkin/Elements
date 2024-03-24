using UnityEngine;
using Zenject;

namespace DefaultNamespace.Installers
{
	public class ProjectInstaller : MonoInstaller
	{
		[SerializeField] private GameSettingsConfig _gameSettingsConfig;
		
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<CommandsProcessor>().AsSingle();
			Container.BindInterfacesTo<GridViewModel>().AsSingle();
			Container.BindInterfacesTo<MoveBlockCommandFactory>().AsSingle();

			Container.BindInterfacesTo<AddressableAssetsLoader>().AsTransient();
			Container.BindInterfacesTo<GridMovementProcessor>().AsSingle();
			Container.BindInterfacesTo<PlayfieldCanvasViewModel>().AsSingle();
			Container.BindInterfacesTo<AnimationsProcessor>().AsSingle();
			Container.BindInterfacesTo<LevelWinObserver>().AsSingle();
			Container.BindInterfacesTo<SaveRestoreDataProcessor>().AsSingle();
			Container.BindInterfacesTo<SaveRestoreDataObserver>().AsSingle();
			Container.BindInterfacesTo<LevelIndexProvider>().AsSingle();
			Container.BindInterfacesTo<ResetPlayfieldNotifier>().AsSingle();

			Container.BindFactory<LevelController, LevelControllerFactory>().AsSingle();

			Container.Bind<IGameSettingsConfigProvider>().To<GameSettingsConfigProvider>()
				.FromInstance(new GameSettingsConfigProvider(_gameSettingsConfig)).AsSingle();

			Container.BindIFactory<GameRunnerController>().To<GameRunnerController>().AsSingle();
			Container.BindIFactory<PlayfieldLoaderController>().To<PlayfieldLoaderController>().AsSingle();
			Container.BindIFactory<LevelController>().To<LevelController>().AsSingle();
		}
	}
}