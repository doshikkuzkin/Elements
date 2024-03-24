using UnityEngine;
using Views;

namespace Data
{
	public struct BlockMoveInfo
	{
		public Vector2Int StartBlockPosition { get; private set; }
		public Vector2Int Direction { get; private set; }

		public BlockMoveInfo(Vector2Int startBlockPosition, Vector2Int direction)
		{
			StartBlockPosition = startBlockPosition;
			Direction = direction;
		}
	}
}