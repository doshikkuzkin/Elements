using System.Threading;
using Assressables;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Observers;
using Processors;
using UnityEngine;
using Views;
using Views.ViewModels;
using Zenject;
using Object = UnityEngine.Object;

namespace Controllers
{
	public class LevelController : Controller
	{
		private readonly ICommandsProcessor _commandsProcessor;
		private readonly IGridMovementProcessor _gridMovementProcessor;
		private readonly IAddressableAssetsLoader _addressableAssetsLoader;
		private readonly IPlayfieldCanvasViewModel _playfieldCanvasViewModel;
		private readonly IResetPlayfieldObserver _resetPlayfieldObserver;
		private readonly IAnimationsProcessor _animationsProcessor;
		private readonly ILevelWinObserver _levelWinObserver;
		private readonly IGridViewModel _gridViewModel;
		private readonly IFactory<PlayfieldLoaderController> _playfieldLoaderFactory;
		
		private PlayfieldLoaderController _playfieldLoaderController;
		private PlayfieldCanvasView _playfieldCanvasView;
		private CancellationTokenSource _playfieldUpdateTokenSource;
		private CancellationToken _gameCancellationToken;
		
		private bool _isDisposed;

		public LevelController(
			ICommandsProcessor commandsProcessor,
			IGridMovementProcessor gridMovementProcessor,
			IAddressableAssetsLoader addressableAssetsLoader,
			IPlayfieldCanvasViewModel playfieldCanvasViewModel,
			IResetPlayfieldObserver resetPlayfieldObserver,
			IAnimationsProcessor animationsProcessor,
			ILevelWinObserver levelWinObserver,
			IGridViewModel gridViewModel,
			IFactory<PlayfieldLoaderController> playfieldLoaderFactory)
		{
			_commandsProcessor = commandsProcessor;
			_gridMovementProcessor = gridMovementProcessor;
			_addressableAssetsLoader = addressableAssetsLoader;
			_playfieldCanvasViewModel = playfieldCanvasViewModel;
			_resetPlayfieldObserver = resetPlayfieldObserver;
			_animationsProcessor = animationsProcessor;
			_levelWinObserver = levelWinObserver;
			_gridViewModel = gridViewModel;
			_playfieldLoaderFactory = playfieldLoaderFactory;
		}

		public override UniTask Initialize(CancellationToken cancellationToken)
		{
			return LoadPlayfield(cancellationToken);
		}

		public override UniTask Execute(CancellationToken cancellationToken)
		{
			_gameCancellationToken = cancellationToken;
			RecreateUpdateToken(cancellationToken);
			UpdatePlayfieldState(_playfieldUpdateTokenSource.Token).Forget();
			
			return UniTask.CompletedTask;
		}

		public override async UniTask Stop(CancellationToken cancellationToken)
		{
			_playfieldUpdateTokenSource?.Cancel();
			_playfieldUpdateTokenSource?.Dispose();
			
			_resetPlayfieldObserver.PlayfieldReset -= OnPlayfieldReset;
			
			await _playfieldLoaderController.Stop(cancellationToken);

			Object.Destroy(_playfieldCanvasView.gameObject);
			_playfieldCanvasViewModel?.Dispose();
			
			_addressableAssetsLoader.UnloadAssets();
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

		private async UniTask LoadPlayfield(CancellationToken cancellationToken)
		{
			await LoadPlayfieldCanvasView(cancellationToken);

			_playfieldLoaderController = _playfieldLoaderFactory.Create();
			
			await _playfieldLoaderController.Initialize(cancellationToken);
			await _playfieldLoaderController.Execute(cancellationToken);
			
			_resetPlayfieldObserver.PlayfieldReset += OnPlayfieldReset;
		}
		
		private async UniTask LoadPlayfieldCanvasView(CancellationToken cancellationToken)
		{
			var canvasPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("PlayfieldCanvas", cancellationToken);
			_playfieldCanvasView = Object.Instantiate(canvasPrefab).GetComponent<PlayfieldCanvasView>();
			
			_playfieldCanvasViewModel.SetView(_playfieldCanvasView);
		}
		
		private void OnPlayfieldReset()
		{
			RecreateUpdateToken(_gameCancellationToken);
			UpdatePlayfieldState(_playfieldUpdateTokenSource.Token).Forget();
		}
	}
}