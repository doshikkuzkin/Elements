using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
	public class MoveBlockCommand : ICommand
	{
		private static bool IsAnimationInProcess = false;
		
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

		public void Execute(CancellationToken cancellationToken)
		{
			if (!IsValidMovement(_cellToMove, _moveDirection))
			{
				return;
			}
			
			var targetCellPosition = _cellToMove + _moveDirection;
				
			_gridViewModel.SwapCells(_cellToMove, targetCellPosition);

			var blockMoveAnimationStep = GetBlockMoveAnimationStep(_cellToMove, targetCellPosition, _moveDirection);
			var normalizeAnimationSteps = new List<NormalizeGridAnimationStep>();

			while (TryNormalize(out var moveAnimationSteps, out var blocksDestroyAnimationStep))
			{
				normalizeAnimationSteps.Add(new NormalizeGridAnimationStep(moveAnimationSteps, blocksDestroyAnimationStep));
			}
			
			PlayAnimationSequence(blockMoveAnimationStep, normalizeAnimationSteps.ToArray(), cancellationToken).Forget();
		}

		private async UniTaskVoid PlayAnimationSequence(BlockMoveAnimationStep blockMoveAnimationStep, NormalizeGridAnimationStep[] normalizeAnimationSteps, CancellationToken cancellationToken)
		{
			var disabledBlocks = DisableMovingBlocks(blockMoveAnimationStep, normalizeAnimationSteps);
			
			await PlaySwapCellsAnimation(blockMoveAnimationStep, cancellationToken);
			
			await UniTask.WaitUntil(() => IsAnimationInProcess == false, cancellationToken: cancellationToken);
			
			IsAnimationInProcess = true;

			try
			{
				foreach (var normalizeAnimationStep in normalizeAnimationSteps)
				{
					if (normalizeAnimationStep.MoveAnimationSteps != null)
					{
						var moveAnimationsList = normalizeAnimationStep.MoveAnimationSteps.Select(moveAnimationStep => PlaySwapCellsAnimation(moveAnimationStep, cancellationToken));

						await UniTask.WhenAll(moveAnimationsList);
					}

					var blocksToDestroyPositions =
						normalizeAnimationStep.BlocksDestroyAnimationStep.BlocksToDestroyPositions?.ToArray();
				
					if (blocksToDestroyPositions != null)
					{
						await blocksToDestroyPositions
							.Select(cellPosition =>
							{
								_gridViewModel.TryGetBlockView(cellPosition, out var blockView);

								return blockView.DestroyBlock(cancellationToken);
							});
					
						_gridViewModel.DestroyCellsViews(blocksToDestroyPositions);
					}
				}

				foreach (var disabledBlock in disabledBlocks)
				{
					disabledBlock.SetIsAllowedToMove(true);
				}
			}
			finally
			{
				IsAnimationInProcess = false;
			}
		}
		
		private List<BlockView> DisableMovingBlocks(BlockMoveAnimationStep blockMoveAnimationStep, IEnumerable<NormalizeGridAnimationStep> normalizeAnimationSteps)
		{
			var animatedCells = new HashSet<Vector2Int>();

			foreach (var blockMoveInfo in blockMoveAnimationStep.BlockMoveInfo)
			{
				animatedCells.Add(blockMoveInfo.StartBlockPosition);
			}

			foreach (var normalizeAnimationStep in normalizeAnimationSteps)
			{
				if (normalizeAnimationStep.MoveAnimationSteps != null)
				{
					foreach (var moveAnimationStep in normalizeAnimationStep.MoveAnimationSteps)
					{
						foreach (var blockMoveInfo in moveAnimationStep.BlockMoveInfo)
						{
							animatedCells.Add(blockMoveInfo.StartBlockPosition);
						}
					}
				}

				var cellsToDestroy = normalizeAnimationStep.BlocksDestroyAnimationStep.BlocksToDestroyPositions;
				
				if (cellsToDestroy != null)
				{
					foreach (var cell in cellsToDestroy)
					{
						animatedCells.Add(cell);
					}
				}
			}

			var disabledBlocks = new List<BlockView>();

			foreach (var cell in animatedCells)
			{
				if (_gridViewModel.TryGetBlockView(cell, out var blockView))
				{
					blockView.SetIsAllowedToMove(false);
					disabledBlocks.Add(blockView);
				}
			}

			return disabledBlocks;
		}

		private async UniTask PlaySwapCellsAnimation(BlockMoveAnimationStep blockMoveAnimationStep, CancellationToken cancellationToken)
		{
			var blockViews = blockMoveAnimationStep.BlockMoveInfo
				.Select(blockMoveInfo => _gridViewModel.TryGetBlockView(blockMoveInfo.StartBlockPosition, out var blockView)
				? blockView : null).ToArray();
			
			var firstCellToSwap = blockMoveAnimationStep.BlockMoveInfo[0].StartBlockPosition;
			var secondCellToSwap = firstCellToSwap + blockMoveAnimationStep.BlockMoveInfo[0].Direction;
			
			_gridViewModel.SwapCellsViews(firstCellToSwap, secondCellToSwap);
			
			await UniTask.WhenAll(blockMoveAnimationStep.BlockMoveInfo.Select((blockMoveInfo, index) =>
			{
				if (blockViews[index] == null)
				{
					return UniTask.CompletedTask;
				}
				
				var newPosition = blockViews[index].transform.localPosition + new Vector3(
					blockMoveInfo.Direction.x * _gridViewModel.CellSize, blockMoveInfo.Direction.y * _gridViewModel.CellSize,
					0);

				return blockViews[index].MoveBlock(newPosition, cancellationToken);
			}));
		}

		private bool TryNormalize(out IEnumerable<BlockMoveAnimationStep> moveAnimationSteps, out BlocksDestroyAnimationStep blocksDestroyAnimationStep)
		{
			moveAnimationSteps = null;
			blocksDestroyAnimationStep = default;
			
			var isNormalized = false;
			
			if (TryGetColumnsToMove(out var columnsToMove))
			{
				moveAnimationSteps = MoveColumns(columnsToMove);
				
				isNormalized = true;
			}

			if (TryGetCellsToDestroy(out var cellsToDestroy))
			{
				blocksDestroyAnimationStep = new BlocksDestroyAnimationStep(cellsToDestroy.Select(cell => cell.Position));
				
				DestroyCells(cellsToDestroy);
				
				isNormalized = true;
			}

			return isNormalized;
		}

		private IEnumerable<BlockMoveAnimationStep> MoveColumns(List<ColumnModel> columnsToMove)
		{
			var blockMoveAnimationSteps = new List<BlockMoveAnimationStep>();
			
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
					
					blockMoveAnimationSteps.Add(GetBlockMoveAnimationStep(cellToMove, firstEmptyCellPosition, Vector2Int.down));
				}
			}

			return blockMoveAnimationSteps;
		}
		
		private void DestroyCells(List<CellModel> cellsToDestroy)
		{
			foreach (var cell in cellsToDestroy)
			{
				cell.SetBlockType(BlockType.None);
			}
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
					
					continue;
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

		private BlockMoveAnimationStep GetBlockMoveAnimationStep(Vector2Int cellToMove, Vector2Int targetCellPosition, Vector2Int moveDirection)
		{
			_gridViewModel.TryGetBlockView(cellToMove, out var blockToMove);
			_gridViewModel.TryGetBlockView(targetCellPosition, out var blockToSwapWith);

			var cellToSwapWith = _gridViewModel.GridModel.Grid[targetCellPosition.x].Cells[targetCellPosition.y];
			
			var distanceBetweenCells = new Vector2Int(Math.Abs(targetCellPosition.x - cellToMove.x),
				Math.Abs(targetCellPosition.y - cellToMove.y));
			distanceBetweenCells *= moveDirection;

			if (cellToSwapWith.BlockType != BlockType.None)
			{
				var blockToSwapWithDirection = distanceBetweenCells * -1;

				return new BlockMoveAnimationStep(
					new BlockMoveInfo(blockToMove, cellToMove, distanceBetweenCells),
					new BlockMoveInfo(blockToSwapWith, cellToSwapWith.Position, blockToSwapWithDirection)
				);
			}
			
			return new BlockMoveAnimationStep(
				new BlockMoveInfo(blockToMove, cellToMove, distanceBetweenCells)
			);
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
			
			if (_gridViewModel.TryGetBlockView(newBlockPosition, out var blockView))
			{
				return blockView.IsAllowedToMove;
			}

			return true;
		}
	}
}