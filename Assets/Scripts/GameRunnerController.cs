using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
	public class GameRunnerController : IGameRunnerController
	{
		private readonly LevelControllerFactory _levelControllerFactory;
		private readonly IPlayfieldCanvasViewModel _playfieldCanvasViewModel;

		private ILevelController _levelController;
		private CancellationTokenSource _levelCancellationSource;
		private CancellationToken _gameCancellationToken;
		private GameSettingsConfig _gameSettingsConfig;
		private int _levelIndex;
		
		private bool _isDisposed;

		public GameRunnerController(
			LevelControllerFactory levelControllerFactory,
			IPlayfieldCanvasViewModel playfieldCanvasViewModel)
		{
			_levelControllerFactory = levelControllerFactory;
			_playfieldCanvasViewModel = playfieldCanvasViewModel;
		}
		
		public async UniTask Execute(GameSettingsConfig gameSettingsConfig, CancellationToken cancellationToken)
		{
			_gameSettingsConfig = gameSettingsConfig;
			_gameCancellationToken = cancellationToken;
			_levelController = _levelControllerFactory.Create();
			
			RecreateLevelCancellationSource(cancellationToken);
			
			var savedLevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
			
			await RunGame(savedLevelIndex, _levelCancellationSource.Token);
			
			_playfieldCanvasViewModel.NextClicked += OnNextClicked;
		}

		private async UniTask RunGame(int levelIndex, CancellationToken cancellationToken)
		{
			_levelIndex = levelIndex;
			
			PlayerPrefs.SetInt("LevelIndex", _levelIndex);
			
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
			_playfieldCanvasViewModel.NextClicked -= OnNextClicked;
			
			_levelCancellationSource?.Cancel();
			_levelCancellationSource?.Dispose();
			
			_levelController?.Dispose();
		}
		
		private void OnNextClicked()
		{
			RecreateLevelCancellationSource(_gameCancellationToken);
			_levelController.Dispose();

			_levelController = _levelControllerFactory.Create();
			
			var nextLevelIndex = _levelIndex + 1 >= _gameSettingsConfig.LevelsCount ? 0 : _levelIndex + 1;

			RunGame(nextLevelIndex, _levelCancellationSource.Token).Forget();
		}

		private void RecreateLevelCancellationSource(CancellationToken cancellationToken)
		{
			_levelCancellationSource?.Cancel();

			_levelCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		}
	}
}