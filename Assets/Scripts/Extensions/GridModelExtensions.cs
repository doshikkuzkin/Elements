using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
	public static class GridModelExtensions
	{
		private static Vector2Int[] _directionsToCheck = new Vector2Int[]
		{
			Vector2Int.up,
			Vector2Int.down,
			Vector2Int.left,
			Vector2Int.right
		};

		private static HashSet<Vector2Int> _checkedPositions = new();

		public static bool CheckCellsSequenceExisting(this GridModel gridModel, Vector2Int targetCellPosition, int targetBlockType, Vector2Int? excludeDirection = null)
		{
			var connectedDirections = new HashSet<Vector2Int>();
			_checkedPositions.Add(targetCellPosition);

			foreach (var direction in _directionsToCheck)
			{
				if (gridModel.TryGetConnectedCellInDirection(targetCellPosition, direction, targetBlockType))
				{
					connectedDirections.Add(direction);
				}
			}

			if (connectedDirections.Count <= 1)
			{
				return false;
			}

			var verticalConnections = 0;
			var horizontalConnections = 0;

			foreach (var direction in connectedDirections)
			{
				if (direction.x == 0)
				{
					verticalConnections++;
				}
				else
				{
					horizontalConnections++;
				}
			}

			if (verticalConnections == 2 || horizontalConnections == 2)
			{
				return true;
			}

			foreach (var direction in connectedDirections)
			{
				if (excludeDirection == null || direction != excludeDirection)
				{
					var newPosition = targetCellPosition + direction;

					if (_checkedPositions.Contains(newPosition))
					{
						continue;
					}

					if (gridModel.CheckCellsSequenceExisting(newPosition, targetBlockType, -direction))
					{
						_checkedPositions.Clear();

						return true;
					}
				}
			}

			_checkedPositions.Clear();
			return false;
		}

		private static bool TryGetConnectedCellInDirection(this GridModel gridModel, Vector2Int cellPosition, Vector2Int direction, int targetBlockType)
		{
			var cellToCheckPosition = cellPosition + direction;

			if (!gridModel.IsValidCellPosition(cellToCheckPosition)) return false;

			var cellToCheck = gridModel.Grid[cellToCheckPosition.x].Cells[cellToCheckPosition.y];

			if (cellToCheck.IsConnected)
			{
				return false;
			}

			if (cellToCheck.BlockType != targetBlockType)
			{
				return false;
			}

			return true;
		}
	}
}
