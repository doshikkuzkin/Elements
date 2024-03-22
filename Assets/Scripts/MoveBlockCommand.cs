using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
	public class MoveBlockCommand : ICommand
	{
		private const int CellsInRowToDestroy = 3;
		
		private readonly Vector2Int _cellToMove;
		private readonly Vector2Int _moveDirection;
		private readonly IGridViewModel _gridViewModel;

		public MoveBlockCommand(Vector2Int cellToMove, Vector2Int moveDirection, IGridViewModel gridViewModel)
		{
			_cellToMove = cellToMove;
			_moveDirection = moveDirection;
			_gridViewModel = gridViewModel;
		}

		public void Execute()
		{
			if (!IsValidMovement(_cellToMove, _moveDirection))
			{
				return;
			}
			
			var targetCellPosition = _cellToMove + _moveDirection;
				
			MoveBlock(targetCellPosition);
				
			MoveCellsViews(_cellToMove, targetCellPosition, _moveDirection);

			while (TryNormalize())
			{
				continue;
			}
		}

		private bool TryNormalize()
		{
			if (TryGetColumnsToMove(out var columnsToMove))
			{
				MoveColumns(columnsToMove);
				
				return true;
			}

			if (TryGetCellsToDestroy(out var cellsToDestroy))
			{
				DestroyCells(cellsToDestroy);
				
				return true;
			}

			return false;
		}

		private void MoveColumns(List<ColumnModel> columnsToMove)
		{
			foreach (var column in columnsToMove)
			{
				for (int i = 1; i < column.Cells.Length; i++)
				{
					if (column.Cells[i].BlockType == BlockType.None || column.Cells[i - 1].BlockType != BlockType.None)
					{
						continue;
					}

					var firstEmptyCell = column.Cells.First(cell => cell.BlockType == BlockType.None);

					var cellToMove = column.Cells[i].Position;
					var firstEmptyCellPosition = firstEmptyCell.Position;
					
					_gridViewModel.SwapCells(cellToMove, firstEmptyCellPosition);
					
					MoveCellsViews(cellToMove, firstEmptyCellPosition, Vector2Int.down);
				}
			}
		}
		
		private void DestroyCells(List<CellModel> cellsToDestroy)
		{
			foreach (var cell in cellsToDestroy)
			{
				cell.SetBlockType(BlockType.None);
			}

			DestroyCellsViews(cellsToDestroy);
		}

		private bool TryGetColumnsToMove(out List<ColumnModel> columnsToMove)
		{
			var grid = _gridViewModel.GridModel.Grid;

			columnsToMove = grid.Where(column =>
					column.Cells.Where(cell => cell.Position.y > 0)
						.Any(cell => cell.BlockType != BlockType.None && _gridViewModel.IsEmptyCell(new Vector2Int(cell.Position.x, cell.Position.y - 1))))
				.ToList();

			return columnsToMove.Any();
		}
		
		private bool TryGetCellsToDestroy(out List<CellModel> cellsToDestroy)
		{
			var grid = _gridViewModel.GridModel.Grid;

			cellsToDestroy = new List<CellModel>();

			var connectedCellsGroups = new List<List<CellModel>>();

			foreach (var column in grid)
			{
				foreach (var cell in column.Cells)
				{
					if (cell.BlockType == BlockType.None)
					{
						continue;
					}

					if (cell.IsConnected)
					{
						continue;
					}

					if (TryGetConnectedCellsGroup(cell.Position, cell.BlockType, out var connectedCells))
					{
						connectedCellsGroups.Add(connectedCells);
					}
				}
			}

			foreach (var cell in connectedCellsGroups.SelectMany(cellsGroup => cellsGroup))
			{
				cell.SetIsConnected(false);
			}

			foreach (var cellsGroup in connectedCellsGroups)
			{
				if (cellsGroup.Count < CellsInRowToDestroy)
				{
					continue;
				}
				
				var cellsInRow = 1;
				var horizontalGroup = cellsGroup.OrderBy(cell => cell.Row).ThenBy(cell => cell.Column).ToArray();
				
				for (var i = 1; i < horizontalGroup.Length; i++)
				{
					if (horizontalGroup[i].Row == horizontalGroup[i - 1].Row && horizontalGroup[i].Column == horizontalGroup[i - 1].Column + 1)
					{
						cellsInRow++;
					}
					else
					{
						cellsInRow = 1;
					}
					
					if (cellsInRow >= CellsInRowToDestroy)
					{
						break;
					}
				}
				
				if (cellsInRow >= CellsInRowToDestroy)
				{
					cellsToDestroy.AddRange(cellsGroup);
					
					break;
				}
				
				var verticalGroup = cellsGroup.OrderBy(cell => cell.Column).ThenBy(cell => cell.Row).ToArray();
				cellsInRow = 1;
				
				for (var i = 1; i < verticalGroup.Length; i++)
				{
					if (verticalGroup[i].Column == verticalGroup[i - 1].Column && verticalGroup[i].Row == verticalGroup[i - 1].Row + 1)
					{
						cellsInRow++;
					}
					else
					{
						cellsInRow = 1;
					}
					
					if (cellsInRow >= CellsInRowToDestroy)
					{
						break;
					}
				}
				
				if (cellsInRow >= CellsInRowToDestroy)
				{
					cellsToDestroy.AddRange(cellsGroup);
				}
			}

			return cellsToDestroy.Count > 0;
		}

		private bool TryGetConnectedCellsGroup(Vector2Int cellPosition, BlockType targetBlockType, out List<CellModel> connectedCells)
		{
			connectedCells = new List<CellModel>();
			
			GetConnectedCells(cellPosition, targetBlockType, connectedCells);

			return connectedCells.Count > 0;
		}

		private void GetConnectedCells(Vector2Int cellPosition, BlockType targetBlockType, List<CellModel> targetList)
		{
			GetConnectedCellsInDirection(cellPosition, Vector2Int.up, targetBlockType, targetList);
			GetConnectedCellsInDirection(cellPosition, Vector2Int.down, targetBlockType, targetList);
			GetConnectedCellsInDirection(cellPosition, Vector2Int.left, targetBlockType, targetList);
			GetConnectedCellsInDirection(cellPosition, Vector2Int.right, targetBlockType, targetList);
		}

		private void GetConnectedCellsInDirection(Vector2Int cellPosition, Vector2Int direction, BlockType targetBlockType, List<CellModel> targetList)
		{
			if (!IsTargetCellInsideGrid(cellPosition, direction))
			{
				return;
			}

			var cellToCheckPosition = cellPosition + direction;

			var cellToCheck = _gridViewModel.GridModel.Grid[cellToCheckPosition.x].Cells[cellToCheckPosition.y];

			if (cellToCheck.IsConnected)
			{
				return;
			}

			if (cellToCheck.BlockType != targetBlockType)
			{
				return;
			}
			
			cellToCheck.SetIsConnected(true);
			targetList.Add(cellToCheck);

			GetConnectedCells(cellToCheckPosition, targetBlockType, targetList);
		}

		private void MoveBlock(Vector2Int targetCellPosition)
		{
			_gridViewModel.SwapCells(_cellToMove, targetCellPosition);
		}

		private void DestroyCellsViews(List<CellModel> cellsToDestroy)
		{
			_gridViewModel.DestroyCellsViews(cellsToDestroy);
		}

		private void MoveCellsViews(Vector2Int cellToMove, Vector2Int targetCellPosition, Vector2Int moveDirection)
		{
			_gridViewModel.TryGetBlockView(cellToMove, out var blockToMove);
			_gridViewModel.TryGetBlockView(targetCellPosition, out var blockToSwapWith);
				
			var startBlockPosition = _gridViewModel.GetCellPositionLocal(blockToMove.CellModel.Position.x, blockToMove.CellModel.Position.y);
				
			blockToMove.MoveBlock(startBlockPosition, moveDirection, _gridViewModel.CellSize);

			if (blockToSwapWith != null)
			{
				var startSwapBlockPosition = _gridViewModel.GetCellPositionLocal(blockToSwapWith.CellModel.Position.x, blockToSwapWith.CellModel.Position.y);
					
				blockToSwapWith.MoveBlock(startSwapBlockPosition, moveDirection * -1, _gridViewModel.CellSize);
			}
				
			_gridViewModel.SwapCellsViews(cellToMove, targetCellPosition);
		}
		
		private bool IsValidMovement(Vector2Int cellToMove, Vector2Int moveDirection)
		{
			return IsTargetCellInsideGrid(cellToMove, moveDirection) &&
			       IsValidDirection(cellToMove, moveDirection);
		}
		
		private bool IsTargetCellInsideGrid(Vector2Int cellToMove, Vector2Int moveDirection)
		{
			var newBlockPosition = cellToMove + moveDirection;
			
			return _gridViewModel.IsValidCellPosition(newBlockPosition);
		}

		private bool IsValidDirection(Vector2Int cellToMove, Vector2Int moveDirection)
		{
			var newBlockPosition = cellToMove + moveDirection;
			
			if (moveDirection == Vector2Int.up)
			{
				return !_gridViewModel.IsEmptyCell(newBlockPosition);
			}

			return true;
		}
	}
}