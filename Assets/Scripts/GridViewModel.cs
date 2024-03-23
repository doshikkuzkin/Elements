using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
	public class GridViewModel : IGridViewModel
	{
		private GridModel _gridModel;
		private GridView _gridView;
		private BlockView[][] _blockViews;
		
		private float _gridWidth;
		private float _startGridPositionX;
		private float _startGridPositionY;

		public BlockView[][] BlockViews => _blockViews;
		public Transform GridParent => _gridView.GridParent;

		public GridModel GridModel => _gridModel;

		public float CellSize => _gridView.GridCellSize;

		public void InitGrid(GridModel gridModel, GridView gridView)
		{
			_gridModel = gridModel;
			_gridView = gridView;
			
			_gridWidth = _gridView.GridCellSize * gridModel.Grid.Length;
			
			var pivotPosition = _gridView.GridParent.position;
			_startGridPositionX = pivotPosition.x - _gridWidth / 2 + _gridView.GridCellSize / 2;
			_startGridPositionY = pivotPosition.y;
		}

		public void InitBlocks(BlockView[][] blockViews)
		{
			_blockViews = blockViews;
		}
		
		public void ApplyScaleFactor()
		{
			_gridView.GridParent.localScale = GetScaleFactor();
		}

		public void ResetScaleFactor()
		{
			_gridView.GridParent.localScale = Vector3.one;
		}

		public Vector3 GetCellPosition(int x, int y)
		{
			return new Vector3(_startGridPositionX + x * _gridView.GridCellSize, _startGridPositionY + y * _gridView.GridCellSize, 0);
		}

		public Vector3 GetCellPositionLocal(int x, int y)
		{
			var startGridPositionX = 0 - _gridWidth / 2 + _gridView.GridCellSize / 2;
			var startGridPositionY = 0;
			
			return new Vector3(startGridPositionX + x * _gridView.GridCellSize, startGridPositionY + y * _gridView.GridCellSize, 0);
		}
		
		public bool TryGetBlockView(Vector2Int cellPosition, out BlockView blockView)
		{
			blockView = _blockViews[cellPosition.x][cellPosition.y];
			
			return blockView != null;
		}
		
		public bool IsValidCellPosition(Vector2Int cellPosition)
		{
			if (cellPosition.x < 0 || cellPosition.y < 0)
			{
				return false;
			}

			if (cellPosition.x >= _gridModel.Grid.Length ||
			    cellPosition.y >= _gridModel.Grid[cellPosition.x].Cells.Length)
			{
				return false;
			}

			return true;
		}

		public bool IsEmptyCell(Vector2Int cellPosition)
		{
			return _gridModel.Grid[cellPosition.x].Cells[cellPosition.y].BlockType == BlockType.None;
		}

		public void SwapCells(Vector2Int firstCellPosition, Vector2Int secondCellPosition)
		{
			var firstCell = _gridModel.Grid[firstCellPosition.x].Cells[firstCellPosition.y];
			var firstCellType = firstCell.BlockType;
			var secondCell = _gridModel.Grid[secondCellPosition.x].Cells[secondCellPosition.y];
			var secondCellType = secondCell.BlockType;
			
			firstCell.SetBlockType(secondCellType);
			secondCell.SetBlockType(firstCellType);
		}

		public void SwapCellsViews(Vector2Int firstCellPosition, Vector2Int secondCellPosition)
		{
			var firstCell = _gridModel.Grid[firstCellPosition.x].Cells[firstCellPosition.y];
			var secondCell = _gridModel.Grid[secondCellPosition.x].Cells[secondCellPosition.y];
			
			if (TryGetBlockView(firstCellPosition, out var firstBlockView))
			{
				firstBlockView.SetCellModel(secondCell);
			}
			
			if (TryGetBlockView(secondCellPosition, out var secondBlockView))
			{
				secondBlockView.SetCellModel(firstCell);
			}
			
			(_blockViews[firstCellPosition.x][firstCellPosition.y],
					_blockViews[secondCellPosition.x][secondCellPosition.y]) =
				(_blockViews[secondCellPosition.x][secondCellPosition.y],
					_blockViews[firstCellPosition.x][firstCellPosition.y]);
		}

		public void DestroyCellsViews(IEnumerable<Vector2Int> cellsToDestroy)
		{
			foreach (var cell in cellsToDestroy)
			{
				if (!TryGetBlockView(cell, out var cellView))
				{
					continue;
				}
				
				cellView.gameObject.SetActive(false);
				_blockViews[cell.x][cell.y] = null;
			}
		}

		private Vector3 GetScaleFactor()
		{
			return _gridWidth > _gridView.MaxGridWidth ? Vector3.one * (_gridView.MaxGridWidth / _gridWidth) : Vector3.one;
		}
	}
}