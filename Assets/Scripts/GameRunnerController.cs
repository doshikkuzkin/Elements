using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public class GameRunnerController : IGameRunnerController
	{
		private readonly LevelControllerFactory _levelControllerFactory;
		private readonly IPlayfieldCanvasViewModel _playfieldCanvasViewModel;
		private readonly ILevelWinObserver _levelWinObserver;
		private readonly ISaveRestoreDataObserver _saveRestoreDataObserver;
		private readonly ILevelIndexProvider _levelIndexProvider;

		private ILevelController _levelController;
		private CancellationTokenSource _levelCancellationSource;
		private CancellationToken _gameCancellationToken;
		
		private bool _isDisposed;

		public GameRunnerController(
			LevelControllerFactory levelControllerFactory,
			IPlayfieldCanvasViewModel playfieldCanvasViewModel,
			ILevelWinObserver levelWinObserver,
			ISaveRestoreDataObserver saveRestoreDataObserver,
			ILevelIndexProvider levelIndexProvider)
		{
			_levelControllerFactory = levelControllerFactory;
			_playfieldCanvasViewModel = playfieldCanvasViewModel;
			_levelWinObserver = levelWinObserver;
			_saveRestoreDataObserver = saveRestoreDataObserver;
			_levelIndexProvider = levelIndexProvider;
		}
		
		public async UniTask Execute(CancellationToken cancellationToken)
		{
			_gameCancellationToken = cancellationToken;
			
			_levelController = _levelControllerFactory.Create();
			
			RecreateLevelCancellationSource(cancellationToken);
			
			await RunGame(_levelIndexProvider.CurrentLevelIndex, _levelCancellationSource.Token);
			
			_playfieldCanvasViewModel.NextClicked += OpenNextLevel;
			_levelWinObserver.LevelWin += OpenNextLevel;
		}

		private async UniTask RunGame(int levelIndex, CancellationToken cancellationToken)
		{
			await _levelController.Initialize(levelIndex + 1, cancellationToken);
			await _levelController.Execute(cancellationToken);
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;
			_playfieldCanvasViewModel.NextClicked -= OpenNextLevel;
			_levelWinObserver.LevelWin -= OpenNextLevel;
			
			_levelCancellationSource?.Cancel();
			_levelCancellationSource?.Dispose();
			
			_levelController?.Dispose();
		}
		
		private void OpenNextLevel()
		{
			RecreateLevelCancellationSource(_gameCancellationToken);
			_levelController.Dispose();

			_levelController = _levelControllerFactory.Create();

			var levelToLoad = _levelIndexProvider.NextLevelIndex;
			_levelIndexProvider.IncrementLevelIndex();
			
			_saveRestoreDataObserver.RequestClear();
			
			RunGame(levelToLoad, _levelCancellationSource.Token).Forget();
		}

		private void RecreateLevelCancellationSource(CancellationToken cancellationToken)
		{
			_levelCancellationSource?.Cancel();

			_levelCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		}
	}
}