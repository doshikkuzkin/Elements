using UnityEngine;
using Views;

namespace Data
{
	public struct BlockMoveInfo
	{
		public BlockView BlockView { get; private set; }
		public Vector2Int StartBlockPosition { get; private set; }
		public Vector2Int Direction { get; private set; }

		public BlockMoveInfo(BlockView blockView, Vector2Int startBlockPosition, Vector2Int direction)
		{
			BlockView = blockView;
			StartBlockPosition = startBlockPosition;
			Direction = direction;
		}
	}
}