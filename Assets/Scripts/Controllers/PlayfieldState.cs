using System.Threading;
using Cysharp.Threading.Tasks;
using Observers;
using Providers;
using States;
using Views.ViewModels;
using Zenject;

namespace Controllers
{
	public class PlayfieldState : StateBase
	{
		private readonly IFactory<PlayfieldState> _gameRunnerControllerFactory;
		private readonly IFactory<LevelController> _levelControllerFactory;
		private readonly ILevelIndexProvider _levelIndexProvider;
		private readonly ILevelStateProvider _levelStateProvider;
		private readonly ILevelWinObserver _levelWinObserver;
		private readonly IPlayfieldCanvasViewModel _playfieldCanvasViewModel;
		private readonly ISaveRestoreDataObserver _saveRestoreDataObserver;
		private CancellationToken _gameCancellationToken;

		private CancellationTokenSource _levelCancellationSource;
		private LevelController _levelController;

		public PlayfieldState(
			IFactory<LevelController> levelControllerFactory,
			IPlayfieldCanvasViewModel playfieldCanvasViewModel,
			ILevelWinObserver levelWinObserver,
			ISaveRestoreDataObserver saveRestoreDataObserver,
			ILevelIndexProvider levelIndexProvider,
			IFactory<PlayfieldState> gameRunnerControllerFactory,
			ILevelStateProvider levelStateProvider)
		{
			_levelControllerFactory = levelControllerFactory;
			_playfieldCanvasViewModel = playfieldCanvasViewModel;
			_levelWinObserver = levelWinObserver;
			_saveRestoreDataObserver = saveRestoreDataObserver;
			_levelIndexProvider = levelIndexProvider;
			_gameRunnerControllerFactory = gameRunnerControllerFactory;
			_levelStateProvider = levelStateProvider;
		}

		public override async UniTask Initialize(CancellationToken cancellationToken)
		{
			_gameCancellationToken = cancellationToken;
			_levelStateProvider.SetIsLevelCleared(false);
			RecreateLevelCancellationSource(cancellationToken);

			await RunGame(_levelCancellationSource.Token);

			await base.Initialize(cancellationToken);
		}

		public override async UniTask<StateResult> Execute(CancellationToken cancellationToken)
		{
			_playfieldCanvasViewModel.NextClicked += OpenNextLevel;
			_levelWinObserver.LevelWin += OpenNextLevel;

			return await base.Execute(cancellationToken);
		}

		public override UniTask Stop(CancellationToken cancellationToken)
		{
			_playfieldCanvasViewModel.NextClicked -= OpenNextLevel;
			_levelWinObserver.LevelWin -= OpenNextLevel;

			_levelCancellationSource?.Cancel();
			_levelCancellationSource?.Dispose();

			return _levelController.Stop(cancellationToken);
		}

		private async UniTask RunGame(CancellationToken cancellationToken)
		{
			_levelController = _levelControllerFactory.Create();

			await _levelController.Initialize(cancellationToken);
			await _levelController.Execute(cancellationToken);
		}

		private void OpenNextLevel()
		{
			RecreateLevelCancellationSource(_gameCancellationToken);

			_levelIndexProvider.IncrementLevelIndex();
			_saveRestoreDataObserver.RequestClear();

			StateCompletionSource.TrySetResult(new StateResult(_gameRunnerControllerFactory.Create()));
		}

		private void RecreateLevelCancellationSource(CancellationToken cancellationToken)
		{
			_levelCancellationSource?.Cancel();
			_levelCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		}
	}
}