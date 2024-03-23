using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
	public class PlayfieldLoader : IPlayfieldLoader
	{
		private readonly IGridViewModel _gridViewModel;
		private readonly IAddressableAssetsLoader _addressableAssetsLoader;
		
		private GameObject _gridPrefab;
		private GameObject _firePrefab;
		private GameObject _waterPrefab;
		private GameObject _backgroundPrefab;
		
		private GridView _gridView;
		private GridModel _gridModel;
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
			DestroyBlocks();
			
			Object.Destroy(_gridView.gameObject);
			Object.Destroy(_backgroundView.gameObject);
			
			_addressableAssetsLoader.UnloadAssets();
		}

		public async UniTask LoadPlayfield(LevelConfig levelConfig, CancellationToken cancellationToken)
		{
			_firePrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("Fire", cancellationToken);
			_waterPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("Water", cancellationToken);
			_gridPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("Grid", cancellationToken);
			_backgroundPrefab = await _addressableAssetsLoader.LoadAsset<GameObject>("Background", cancellationToken);

			_backgroundView = Object.Instantiate(_backgroundPrefab);
			_gridView = Object.Instantiate(_gridPrefab).GetComponent<GridView>();
			
			SpawnGrid(levelConfig);
		}
		
		public void ResetPlayfield(LevelConfig levelConfig)
		{
			DestroyBlocks();
			_gridViewModel.ResetScaleFactor();
			
			SpawnGrid(levelConfig);
		}

		private void DestroyBlocks()
		{
			foreach (var blockViews in _gridViewModel.BlockViews)
			{
				foreach (var blockView in blockViews)
				{
					if (blockView == null)
					{
						continue;
					}
					
					Object.Destroy(blockView.gameObject);
				}
			}
		}

		private void SpawnGrid(LevelConfig levelConfig)
		{
			_gridModel = (GridModel) levelConfig.GridModel.Clone();
			
			_gridViewModel.InitGrid(_gridModel, _gridView);
			
			var blocksViews = new BlockView[_gridModel.Grid.Length][];

			for (int x = 0; x < _gridModel.Grid.Length; x++)
			{
				blocksViews[x] = new BlockView[_gridModel.Grid[x].Cells.Length];
				
				for (int y = 0; y < _gridModel.Grid[x].Cells.Length; y++)
				{
					var cell = _gridModel.Grid[x].Cells[y];
					
					if (TrySpawnPrefab(cell.BlockType, _gridViewModel.GetCellPosition(x, y), out var spawnedObject))
					{
						var blockView = spawnedObject.GetComponent<BlockView>();
						blockView.SetCellModel(cell);
						
						blocksViews[x][y] = blockView;
					}
				}
			}
			
			_gridViewModel.InitBlocks(blocksViews);
			_gridViewModel.ApplyScaleFactor();
		}
		
		private bool TrySpawnPrefab(BlockType blockType, Vector3 position, out GameObject spawnedObject)
		{
			spawnedObject = null;
			
			switch (blockType)
			{
				case BlockType.Fire:
					spawnedObject = Object.Instantiate(_firePrefab, position, Quaternion.identity, _gridViewModel.GridParent);
					
					return true;
				case BlockType.Water:
					spawnedObject = Object.Instantiate(_waterPrefab, position, Quaternion.identity, _gridViewModel.GridParent);
					
					return true;
				default:
					return false;
			}
		}
	}
}