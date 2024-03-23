using Zenject;

namespace DefaultNamespace.Installers
{
	public class ProjectInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<CommandsProcessor>().AsSingle();
			Container.BindInterfacesTo<GridViewModel>().AsSingle();
			Container.BindInterfacesTo<MoveBlockCommandFactory>().AsSingle();

			Container.BindInterfacesTo<AddressableAssetsLoader>().AsTransient();
			Container.BindInterfacesTo<PlayfieldLoader>().AsSingle();
			Container.BindInterfacesTo<GridMovementProcessor>().AsSingle();
			Container.BindInterfacesTo<LevelController>().AsTransient();
			Container.BindInterfacesTo<GameRunnerController>().AsTransient();
			Container.BindInterfacesTo<PlayfieldCanvasViewModel>().AsSingle();
		}
	}
}