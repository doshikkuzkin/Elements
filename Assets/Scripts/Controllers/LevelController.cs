using System.Threading;
using Assressables;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Data;
using Observers;
using Processors;
using Providers;
using UnityEngine;
using Views;
using Views.ViewModels;
using Zenject;

namespace Controllers
{
	public class LevelController : Controller
	{
		private readonly IAddressableAssetsLoader _addressableAssetsLoader;
		private readonly IAnimationsProcessor _animationsProcessor;
		private readonly ICommandsProcessor _commandsProcessor;
		private readonly IGridMovementProcessor _gridMovementProcessor;
		private readonly ILevelStateProvider _levelStateProvider;
		private readonly ILevelWinObserver _levelWinObserver;
		private readonly IPlayfieldCanvasViewModel _playfieldCanvasViewModel;
		private readonly IFactory<PlayfieldLoaderController> _playfieldLoaderFactory;
		private readonly IResetPlayfieldObserver _resetPlayfieldObserver;

		private CancellationToken _gameCancellationToken;

		private bool _isDisposed;
		private PlayfieldCanvasView _playfieldCanvasView;

		private PlayfieldLoaderController _playfieldLoaderController;
		private CancellationTokenSource _playfieldUpdateTokenSource;

		public LevelController(
			ICommandsProcessor commandsProcessor,
			IGridMovementProcessor gridMovementProcessor,
			IAddressableAssetsLoader addressableAssetsLoader,
			IPlayfieldCanvasViewModel playfieldCanvasViewModel,
			IResetPlayfieldObserver resetPlayfieldObserver,
			IAnimationsProcessor animationsProcessor,
			ILevelWinObserver levelWinObserver,
			IFactory<PlayfieldLoaderController> playfieldLoaderFactory,
			ILevelStateProvider levelStateProvider)
		{
			_commandsProcessor = commandsProcessor;
			_gridMovementProcessor = gridMovementProcessor;
			_addressableAssetsLoader = addressableAssetsLoader;
			_playfieldCanvasViewModel = playfieldCanvasViewModel;
			_resetPlayfieldObserver = resetPlayfieldObserver;
			_animationsProcessor = animationsProcessor;
			_levelWinObserver = levelWinObserver;
			_playfieldLoaderFactory = playfieldLoaderFactory;
			_levelStateProvider = levelStateProvider;
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
			
			_animationsProcessor.ClearAnimationsSequence();

			_resetPlayfieldObserver.PlayfieldReset -= OnPlayfieldReset;

			await _playfieldLoaderController.Stop(cancellationToken);

			Object.Destroy(_playfieldCanvasView.gameObject);
			_playfieldCanvasViewModel?.Dispose();

			_addressableAssetsLoader.UnloadAssets();
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
			var canvasPrefab =
				await _addressableAssetsLoader.LoadAsset<GameObject>(AddressablesNames.PlayfieldCanvasName, cancellationToken);
			_playfieldCanvasView = Object.Instantiate(canvasPrefab).GetComponent<PlayfieldCanvasView>();

			_playfieldCanvasViewModel.SetView(_playfieldCanvasView);
		}

		private void OnPlayfieldReset()
		{
			_levelStateProvider.SetIsLevelCleared(false);

			RecreateUpdateToken(_gameCancellationToken);
			_animationsProcessor.ClearAnimationsSequence();
			UpdatePlayfieldState(_playfieldUpdateTokenSource.Token).Forget();
		}
		
		private UniTask UpdatePlayfieldState(CancellationToken cancellationToken)
		{
			return UniTaskAsyncEnumerable.EveryUpdate().ForEachAsync(_ =>
			{
				_gridMovementProcessor.ProcessUserInput();
				_commandsProcessor.ProcessCommands(cancellationToken);

				if (_levelStateProvider.IsLevelCleared && !_animationsProcessor.HasAnimationsInProcess)
					_levelWinObserver.RequestLevelWin();
			}, cancellationToken);
		}
		
		private void RecreateUpdateToken(CancellationToken cancellationToken)
		{
			_playfieldUpdateTokenSource?.Cancel();
			_playfieldUpdateTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		}
	}
}