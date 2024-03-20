using UnityEngine;

namespace DefaultNamespace
{
	public class GridSpawner : MonoBehaviour
	{
		//Use as args
		[SerializeField] private GridView _gridView;
		[SerializeField] private GameObject _firePrefab;
		[SerializeField] private GameObject _waterPrefab;

		public GridModel GridModel { get; private set; }
		public IGridViewModel GridViewModel { get; private set; }

		public void SpawnGrid(GridModel gridModel)
		{
			GridModel = gridModel;
			
			GridViewModel = new GridViewModel();
			GridViewModel.InitGrid(gridModel, _gridView);
			
			var blocksViews = new BlockView[GridModel.Grid.Length][];

			for (int x = 0; x < gridModel.Grid.Length; x++)
			{
				blocksViews[x] = new BlockView[gridModel.Grid[x].Cells.Length];
				
				for (int y = 0; y < gridModel.Grid[x].Cells.Length; y++)
				{
					var cell = gridModel.Grid[x].Cells[y];
					
					if (TrySpawnPrefab(cell.BlockType, GridViewModel.GetCellPosition(x, y), out var spawnedObject))
					{
						var blockView = spawnedObject.GetComponent<BlockView>();
						blockView.SetCellModel(cell);
						
						blocksViews[x][y] = blockView;
					}
				}
			}
			
			GridViewModel.InitBlocks(blocksViews);
			GridViewModel.ApplyScaleFactor();
		}
		
		private bool TrySpawnPrefab(BlockType blockType, Vector3 position, out GameObject spawnedObject)
		{
			spawnedObject = null;
			
			switch (blockType)
			{
				case BlockType.Fire:
					spawnedObject = Instantiate(_firePrefab, position, Quaternion.identity, GridViewModel.GridParent);
					
					return true;
				case BlockType.Water:
					spawnedObject = Instantiate(_waterPrefab, position, Quaternion.identity, GridViewModel.GridParent);
					
					return true;
				default:
					return false;
			}
		}
	}
}