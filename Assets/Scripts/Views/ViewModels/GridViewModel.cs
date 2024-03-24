using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Views.ViewModels
{
	public class GridViewModel : IGridViewModel
	{
		private GridView _gridView;

		private float _gridWidth;
		private float _startGridPositionX;
		private float _startGridPositionY;

		public BlockView[][] BlockViews { get; private set; }

		public GridModel GridModel { get; private set; }

		public float CellSize => _gridView.GridCellSize;

		public void InitGrid(GridModel gridModel, GridView gridView)
		{
			GridModel = gridModel;
			_gridView = gridView;

			_gridWidth = _gridView.GridCellSize * gridModel.Grid.Length;

			var pivotPosition = _gridView.GridParent.position;
			_startGridPositionX = pivotPosition.x - _gridWidth / 2 + _gridView.GridCellSize / 2;
			_startGridPositionY = pivotPosition.y;
		}

		public void InitBlocks(BlockView[][] blockViews)
		{
			BlockViews = blockViews;
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
			return new Vector3(_startGridPositionX + x * _gridView.GridCellSize,
				_startGridPositionY + y * _gridView.GridCellSize, 0);
		}

		public bool TryGetBlockView(Vector2Int cellPosition, out BlockView blockView)
		{
			blockView = BlockViews[cellPosition.x][cellPosition.y];

			return blockView != null;
		}

		public bool IsValidCellPosition(Vector2Int cellPosition)
		{
			if (cellPosition.x < 0 || cellPosition.y < 0)
			{
				return false;
			}

			if (cellPosition.x >= GridModel.Grid.Length ||
			    cellPosition.y >= GridModel.Grid[cellPosition.x].Cells.Length)
			{
				return false;
			}

			return true;
		}

		public bool IsEmptyCell(Vector2Int cellPosition)
		{
			return GridModel.Grid[cellPosition.x].Cells[cellPosition.y].BlockType == BlockTypeData.EmptyBlockType;
		}

		public void SwapCells(Vector2Int firstCellPosition, Vector2Int secondCellPosition)
		{
			var firstCell = GridModel.Grid[firstCellPosition.x].Cells[firstCellPosition.y];
			var firstCellType = firstCell.BlockType;
			var secondCell = GridModel.Grid[secondCellPosition.x].Cells[secondCellPosition.y];
			var secondCellType = secondCell.BlockType;

			firstCell.SetBlockType(secondCellType);
			secondCell.SetBlockType(firstCellType);
		}

		public void SwapCellsViews(Vector2Int firstCellPosition, Vector2Int secondCellPosition)
		{
			var firstCell = GridModel.Grid[firstCellPosition.x].Cells[firstCellPosition.y];
			var secondCell = GridModel.Grid[secondCellPosition.x].Cells[secondCellPosition.y];
			
			if (TryGetBlockView(firstCellPosition, out var firstBlockView))
			{
				firstBlockView.SetCellModel(secondCell);
			}
			
			if (TryGetBlockView(secondCellPosition, out var secondBlockView))
			{
				secondBlockView.SetCellModel(firstCell);
			}
			
			(BlockViews[firstCellPosition.x][firstCellPosition.y],
					BlockViews[secondCellPosition.x][secondCellPosition.y]) =
				(BlockViews[secondCellPosition.x][secondCellPosition.y],
					BlockViews[firstCellPosition.x][firstCellPosition.y]);
		}

		public void DestroyCellsViews(IEnumerable<Vector2Int> cellsToDestroy)
		{
			foreach (var cell in cellsToDestroy)
			{
				if (!TryGetBlockView(cell, out var cellView)) continue;

				cellView.gameObject.SetActive(false);
				BlockViews[cell.x][cell.y] = null;
			}
		}

		public bool AreAllBlocksDestroyed()
		{
			if (GridModel.Grid.SelectMany(column => column.Cells).Any(cell => cell.BlockType != BlockTypeData.EmptyBlockType))
			{
				return false;
			}
			
			return true;
		}

		private Vector3 GetScaleFactor()
		{
			return _gridWidth > _gridView.MaxGridWidth
				? Vector3.one * (_gridView.MaxGridWidth / _gridWidth)
				: Vector3.one;
		}
	}
}