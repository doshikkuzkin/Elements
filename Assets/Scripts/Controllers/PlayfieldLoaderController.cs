using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assressables;
using Cysharp.Threading.Tasks;
using Data;
using Newtonsoft.Json;
using Observers;
using Pooling;
using Providers;
using ScriptableObjects;
using UnityEngine;
using Views;
using Views.ViewModels;

namespace Controllers
{
	public class PlayfieldLoaderController : Controller
	{
		private readonly IAddressableAssetsLoader _addressableAssetsLoader;
		private readonly Dictionary<int, PrefabsPool<BlockView>> _blocksPoolsDictionary = new();
		private readonly IGameSettingsConfigProvider _gameSettingsConfigProvider;

		private readonly IGridViewModel _gridViewModel;
		private readonly ILevelIndexProvider _levelIndexProvider;
		private readonly IPlayfieldCanvasViewModel _playfieldCanvasViewModel;
		private readonly IResetPlayfieldObserver _resetPlayfieldObserver;
		private readonly ISaveRestoreDataObserver _saveRestoreDataObserver;
		private GameObject _backgroundPrefab;
		private GameObject _backgroundView;

		private GridModel _gridModel;

		private GameObject _gridPrefab;
		private GridView _gridView;

		private LevelConfig _levelConfig;

		public PlayfieldLoaderController(
			IGridViewModel gridViewModel,
			IAddressableAssetsLoader addressableAssetsLoader,
			ILevelIndexProvider levelIndexProvider,
			IPlayfieldCanvasViewModel playfieldCanvasViewModel,
			IResetPlayfieldObserver resetPlayfieldObserver,
			ISaveRestoreDataObserver saveRestoreDataObserver,
			IGameSettingsConfigProvider gameSettingsConfigProvider)
		{
			_gridViewModel = gridViewModel;
			_addressableAssetsLoader = addressableAssetsLoader;
			_levelIndexProvider = levelIndexProvider;
			_playfieldCanvasViewModel = playfieldCanvasViewModel;
			_resetPlayfieldObserver = resetPlayfieldObserver;
			_saveRestoreDataObserver = saveRestoreDataObserver;
			_gameSettingsConfigProvider = gameSettingsConfigProvider;
		}

		public override UniTask Initialize(CancellationToken cancellationToken)
		{
			return LoadLevelConfig(cancellationToken);
		}

		public override UniTask Execute(CancellationToken cancellationToken)
		{
			_playfieldCanvasViewModel.ResetClicked += OnResetClicked;

			return LoadPlayfield(cancellationToken);
		}

		public override UniTask Stop(CancellationToken cancellationToken)
		{
			_playfieldCanvasViewModel.ResetClicked -= OnResetClicked;

			ReturnBlocksToPools();

			Object.Destroy(_gridView.gameObject);
			Object.Destroy(_backgroundView.gameObject);

			_addressableAssetsLoader.UnloadAssets();

			return UniTask.CompletedTask;
		}

		private void OnResetClicked()
		{
			_saveRestoreDataObserver.RequestClear();
			ResetPlayfield();

			_resetPlayfieldObserver.NotifyPlayfieldReset();
		}

		private void ResetPlayfield()
		{
			ReturnBlocksToPools();
			_gridViewModel.ResetScaleFactor();

			SpawnGrid((GridModel) _levelConfig.GridModel.Clone());
		}

		private async UniTask LoadLevelConfig(CancellationToken cancellationToken)
		{
			_levelConfig = await _addressableAssetsLoader.LoadAsset<LevelConfig>(
				string.Format(AddressablesNames.LevelConfigKey, _levelIndexProvider.CurrentLevelIndex + 1), cancellationToken);
		}

		private async UniTask LoadPlayfield(CancellationToken cancellationToken)
		{
			var levelRestoreData = PlayerPrefs.GetString(SaveKeys.LevelStateKey, null);

			var initialGridState = string.IsNullOrEmpty(levelRestoreData)
				? (GridModel) _levelConfig.GridModel.Clone()
				: JsonConvert.DeserializeObject<GridModel>(levelRestoreData);


			_gridPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>(AddressablesNames.GridPrefabName, cancellationToken);
			_backgroundPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>(AddressablesNames.BackgroundPrefabName, cancellationToken);

			_backgroundView = Object.Instantiate(_backgroundPrefab);
			_gridView = Object.Instantiate(_gridPrefab).GetComponent<GridView>();

			await InitializePools(cancellationToken);

			SpawnGrid(initialGridState);
		}

		private async UniTask InitializePools(CancellationToken cancellationToken)
		{
			var blocksTypes = _gameSettingsConfigProvider.GameSettingsConfig.BlockTypesData;

			for (var i = 0; i < blocksTypes.Length; i++)
			{
				var prefab =
					await _addressableAssetsLoader.LoadAsset<GameObject>(blocksTypes[i].AddressablesKey,
						cancellationToken);

				_blocksPoolsDictionary.Add(i, new PrefabsPool<BlockView>(prefab));
			}
		}

		private void ReturnBlocksToPools()
		{
			foreach (var blockViews in _gridViewModel.BlockViews)
			{
				var views = blockViews.Where(blockView => blockView != null);
					
				foreach (var view in views)
				{
					_blocksPoolsDictionary[view.BlockType].Return(view);
				}
			}
		}

		private void SpawnGrid(GridModel gridModel)
		{
			_gridModel = gridModel;
			_gridViewModel.InitGrid(_gridModel, _gridView);

			var blocksViews = new BlockView[_gridModel.Grid.Length][];

			for (var x = 0; x < _gridModel.Grid.Length; x++)
			{
				blocksViews[x] = new BlockView[_gridModel.Grid[x].Cells.Length];

				for (var y = 0; y < _gridModel.Grid[x].Cells.Length; y++)
				{
					var cell = _gridModel.Grid[x].Cells[y];

					if (TrySpawnPrefab(cell.BlockType, _gridViewModel.GetCellPosition(x, y), out var blockView))
					{
						blockView.SetCellModel(cell);
						blockView.SetBlockType(cell.BlockType);
						blockView.SetIsAllowedToMove(true);

						blocksViews[x][y] = blockView;
					}
				}
			}

			_gridViewModel.InitBlocks(blocksViews);
			_gridViewModel.ApplyScaleFactor();
		}

		private bool TrySpawnPrefab(int blockType, Vector3 position, out BlockView blockView)
		{
			blockView = null;

			if (!_blocksPoolsDictionary.ContainsKey(blockType))
			{
				return false;
			}
			
			blockView = _blocksPoolsDictionary[blockType].Get(position, _gridView.GridParent);

			return true;
		}
	}
}