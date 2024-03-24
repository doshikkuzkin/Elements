using Assressables;
using Commands;
using Controllers;
using Observers;
using Processors;
using Providers;
using ScriptableObjects;
using States;
using UnityEngine;
using Views;
using Views.ViewModels;
using Zenject;

namespace Installers
{
	public class ProjectInstaller : MonoInstaller
	{
		[SerializeField] private GameSettingsConfig _gameSettingsConfig;
		[SerializeField] private LoaderView _loaderView;
		
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
			Container.BindInterfacesTo<ResetPlayfieldObserver>().AsSingle();

			Container.Bind<IGameSettingsConfigProvider>().To<GameSettingsConfigProvider>()
				.FromInstance(new GameSettingsConfigProvider(_gameSettingsConfig)).AsSingle();

			Container.Bind<ILoaderView>().FromInstance(_loaderView).AsSingle();

			Container.BindIFactory<GameRunnerController>().To<GameRunnerController>().AsSingle();
			Container.BindIFactory<PlayfieldLoaderController>().To<PlayfieldLoaderController>().AsSingle();
			Container.BindIFactory<LevelController>().To<LevelController>().AsSingle();
			Container.BindIFactory<LoaderStateMachineDecorator>().To<LoaderStateMachineDecorator>().AsSingle();
			Container.BindIFactory<RootStateMachine>().To<RootStateMachine>().AsSingle();
		}
	}
}