using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
	public class PlayfieldLoader : IPlayfieldLoader
	{
		private readonly IGridViewModel _gridViewModel;
		private readonly IAddressableAssetsLoader _addressableAssetsLoader;
		private readonly Dictionary<BlockType, PrefabsPool<BlockView>> _blocksPoolsDictionary = new();
		
		private GameObject _gridPrefab;
		private GameObject _backgroundPrefab;
		
		private GridModel _gridModel;
		private GridView _gridView;
		private GameObject _backgroundView;
		
		private bool _isDisposed;

		public PlayfieldLoader(IGridViewModel gridViewModel, IAddressableAssetsLoader addressableAssetsLoader)
		{
			_gridViewModel = gridViewModel;
			_addressableAssetsLoader = addressableAssetsLoader;
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;
			ReturnBlocksToPools();
			
			Object.Destroy(_gridView.gameObject);
			Object.Destroy(_backgroundView.gameObject);
			
			_addressableAssetsLoader.UnloadAssets();
		}

		public async UniTask LoadPlayfield(LevelConfig levelConfig, CancellationToken cancellationToken)
		{
			var levelRestoreData = PlayerPrefs.GetString("LevelState", null);

			var initialGridState = string.IsNullOrEmpty(levelRestoreData)
				? (GridModel) levelConfig.GridModel.Clone()
				: JsonConvert.DeserializeObject<GridModel>(levelRestoreData);
			
			
			_gridPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("Grid", cancellationToken);
			_backgroundPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("Background", cancellationToken);

			_backgroundView = Object.Instantiate(_backgroundPrefab);
			_gridView = Object.Instantiate(_gridPrefab).GetComponent<GridView>();

			await InitializePools(cancellationToken);
			
			SpawnGrid(initialGridState);
		}

		private async UniTask InitializePools(CancellationToken cancellationToken)
		{
			var firePrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("Fire", cancellationToken);
			var waterPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("Water", cancellationToken);
			
			_blocksPoolsDictionary.Add(BlockType.Fire, new PrefabsPool<BlockView>(firePrefab));
			_blocksPoolsDictionary.Add(BlockType.Water, new PrefabsPool<BlockView>(waterPrefab));
		}
		
		public void ResetPlayfield(LevelConfig levelConfig)
		{
			ReturnBlocksToPools();
			_gridViewModel.ResetScaleFactor();
			
			SpawnGrid((GridModel) levelConfig.GridModel.Clone());
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

			for (int x = 0; x < _gridModel.Grid.Length; x++)
			{
				blocksViews[x] = new BlockView[_gridModel.Grid[x].Cells.Length];
				
				for (int y = 0; y < _gridModel.Grid[x].Cells.Length; y++)
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
		
		private bool TrySpawnPrefab(BlockType blockType, Vector3 position, out BlockView blockView)
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