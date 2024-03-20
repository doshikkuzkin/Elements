using UnityEngine;

namespace DefaultNamespace
{
	public class GridSpawner : MonoBehaviour
	{
		//Use as args
		[SerializeField] private Transform _gridPivot;
		[SerializeField] private Transform _gridParent;
		[SerializeField] private float _gridCellSize;
		[SerializeField] private float _maxFieldWidth;
		
		[SerializeField] private GameObject _firePrefab;
		[SerializeField] private GameObject _waterPrefab;

		public void SpawnGrid(GridModel gridModel)
		{
			var gridWidth = _gridCellSize * gridModel.Grid.Length;

			var gridScaleFactor = gridWidth > _maxFieldWidth ? Vector3.one * (_maxFieldWidth / gridWidth) : Vector3.one;
			
			var pivotPosition = _gridParent.position;
			var startGridPositionX = pivotPosition.x - gridWidth / 2 + _gridCellSize / 2;
			var startGridPositionY = pivotPosition.y;

			for (int x = 0; x < gridModel.Grid.Length; x++)
			{
				for (int y = 0; y < gridModel.Grid[x].Cells.Length; y++)
				{
					var cell = gridModel.Grid[x].Cells[y];
					
					SpawnPrefab(cell.BlockType, new Vector3(startGridPositionX + x * _gridCellSize, startGridPositionY + y * _gridCellSize, 0));
				}
			}

			_gridPivot.localScale = gridScaleFactor;
		}
		
		private void SpawnPrefab(BlockType blockType, Vector3 position)
		{
			GameObject prefab;
			
			switch (blockType)
			{
				case BlockType.Fire:
					prefab = _firePrefab;
					break;
				case BlockType.Water:
					prefab = _waterPrefab;
					break;
				default:
					return;
			}

			Instantiate(prefab, position, Quaternion.identity, _gridParent);
		}
	}
}