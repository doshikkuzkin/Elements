using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
	public class GameRunnerController : IGameRunnerController
	{
		private readonly LevelControllerFactory _levelControllerFactory;
		private readonly IPlayfieldCanvasViewModel _playfieldCanvasViewModel;
		private readonly ILevelWinObserver _levelWinObserver;
		private readonly IGameSettingsConfigProvider _gameSettingsConfigProvider;
		private readonly ISaveRestoreDataObserver _saveRestoreDataObserver;

		private ILevelController _levelController;
		private CancellationTokenSource _levelCancellationSource;
		private CancellationToken _gameCancellationToken;
		private int _levelIndex;
		
		private bool _isDisposed;

		public GameRunnerController(
			LevelControllerFactory levelControllerFactory,
			IPlayfieldCanvasViewModel playfieldCanvasViewModel,
			ILevelWinObserver levelWinObserver,
			IGameSettingsConfigProvider gameSettingsConfigProvider,
			ISaveRestoreDataObserver saveRestoreDataObserver)
		{
			_levelControllerFactory = levelControllerFactory;
			_playfieldCanvasViewModel = playfieldCanvasViewModel;
			_levelWinObserver = levelWinObserver;
			_gameSettingsConfigProvider = gameSettingsConfigProvider;
			_saveRestoreDataObserver = saveRestoreDataObserver;
		}
		
		public async UniTask Execute(CancellationToken cancellationToken)
		{
			_gameCancellationToken = cancellationToken;
			
			_levelController = _levelControllerFactory.Create();
			
			RecreateLevelCancellationSource(cancellationToken);
			
			var savedLevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
			
			await RunGame(savedLevelIndex, _levelCancellationSource.Token);
			
			_playfieldCanvasViewModel.NextClicked += OpenNextLevel;
			_levelWinObserver.LevelWin += OpenNextLevel;
		}

		private async UniTask RunGame(int levelIndex, CancellationToken cancellationToken)
		{
			_levelIndex = levelIndex;
			
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

			var nextLevelIndex = _levelIndex + 1;
			var levelToLoad = nextLevelIndex >= _gameSettingsConfigProvider.GameSettingsConfig.LevelsCount ? 0 : nextLevelIndex;

			PlayerPrefs.SetInt("LevelIndex", levelToLoad);
			
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