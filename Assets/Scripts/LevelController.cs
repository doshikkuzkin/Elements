using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
	public class LevelController : ILevelController
	{
		private const string LevelConfigKey = "Level{0}Config";
		
		private IPlayfieldLoader _playfieldLoader;
		private readonly ICommandsProcessor _commandsProcessor;
		private readonly IGridMovementProcessor _gridMovementProcessor;
		private readonly IAddressableAssetsLoader _addressableAssetsLoader;
		private readonly IPlayfieldCanvasViewModel _playfieldCanvasViewModel;
		private readonly IAnimationsProcessor _animationsProcessor;
		private readonly ILevelWinObserver _levelWinObserver;
		private readonly IGridViewModel _gridViewModel;
		private readonly ISaveRestoreDataObserver _saveRestoreDataObserver;
		
		private int _levelIndex;
		private LevelConfig _levelConfig;
		private PlayfieldCanvasView _playfieldCanvasView;
		private CancellationTokenSource _playfieldUpdateTokenSource;
		private CancellationToken _gameCancellationToken;
		
		private bool _isDisposed;

		public LevelController(
			IPlayfieldLoader playfieldLoader,
			ICommandsProcessor commandsProcessor,
			IGridMovementProcessor gridMovementProcessor,
			IAddressableAssetsLoader addressableAssetsLoader,
			IPlayfieldCanvasViewModel playfieldCanvasViewModel,
			IAnimationsProcessor animationsProcessor,
			ILevelWinObserver levelWinObserver,
			IGridViewModel gridViewModel,
			ISaveRestoreDataObserver saveRestoreDataObserver)
		{
			_playfieldLoader = playfieldLoader;
			_commandsProcessor = commandsProcessor;
			_gridMovementProcessor = gridMovementProcessor;
			_addressableAssetsLoader = addressableAssetsLoader;
			_playfieldCanvasViewModel = playfieldCanvasViewModel;
			_animationsProcessor = animationsProcessor;
			_levelWinObserver = levelWinObserver;
			_gridViewModel = gridViewModel;
			_saveRestoreDataObserver = saveRestoreDataObserver;
		}

		public UniTask Initialize(int levelIndex, CancellationToken cancellationToken)
		{
			_levelIndex = levelIndex;

			return LoadPlayfield(cancellationToken);
		}

		public UniTask Execute(CancellationToken cancellationToken)
		{
			_gameCancellationToken = cancellationToken;
			RecreateUpdateToken(cancellationToken);
			
			UpdatePlayfieldState(_playfieldUpdateTokenSource.Token).Forget();
			
			return UniTask.CompletedTask;
		}

		private void RecreateUpdateToken(CancellationToken cancellationToken)
		{
			_playfieldUpdateTokenSource?.Cancel();
			_playfieldUpdateTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		}

		private UniTask UpdatePlayfieldState(CancellationToken cancellationToken)
		{
			return UniTaskAsyncEnumerable.EveryUpdate().ForEachAsync(_ =>
			{
				_gridMovementProcessor.ProcessUserInput();
				_commandsProcessor.ProcessCommands(cancellationToken);

				if (!_animationsProcessor.HasAnimationsInProcess && _gridViewModel.AreAllBlocksDestroyed())
				{
					_levelWinObserver.RequestLevelWin();
				}
			}, cancellationToken);
		}
		
		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;
			
			_playfieldUpdateTokenSource?.Cancel();
			_playfieldUpdateTokenSource?.Dispose();
			_playfieldLoader?.Dispose();
			_playfieldLoader = null;
			
			_playfieldCanvasViewModel.ResetClicked -= OnResetClicked;

			Object.Destroy(_playfieldCanvasView.gameObject);
			_playfieldCanvasViewModel?.Dispose();
			_levelConfig = null;
			
			_addressableAssetsLoader.UnloadAssets();
		}

		private async UniTask LoadPlayfield(CancellationToken cancellationToken)
		{
			await LoadLevelConfig(cancellationToken);
			await LoadPlayfieldCanvasView(cancellationToken);
			
			await _playfieldLoader.LoadPlayfield(_levelConfig, cancellationToken);
			
			_playfieldCanvasViewModel.ResetClicked += OnResetClicked;
		}

		private async UniTask LoadLevelConfig(CancellationToken cancellationToken)
		{
			_levelConfig = await _addressableAssetsLoader.LoadAsset<LevelConfig>(string.Format(LevelConfigKey, _levelIndex), cancellationToken);
		}
		
		private async UniTask LoadPlayfieldCanvasView(CancellationToken cancellationToken)
		{
			var canvasPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("PlayfieldCanvas", cancellationToken);
			_playfieldCanvasView = Object.Instantiate(canvasPrefab).GetComponent<PlayfieldCanvasView>();
			
			_playfieldCanvasViewModel.SetView(_playfieldCanvasView);
		}
		
		private void OnResetClicked()
		{
			RecreateUpdateToken(_gameCancellationToken);
			
			_saveRestoreDataObserver.RequestClear();
			_playfieldLoader.ResetPlayfield(_levelConfig);
			
			UpdatePlayfieldState(_playfieldUpdateTokenSource.Token).Forget();
		}
	}
}