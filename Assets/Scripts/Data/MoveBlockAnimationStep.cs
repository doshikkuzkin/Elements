using UnityEngine;
using Views;

namespace Data
{
	public class MoveBlockAnimationStep
	{
		public MoveBlockAnimationStep(BlockView blockView, Vector3 endPosition)
		{
			BlockView = blockView;
			EndPosition = endPosition;
		}

		public BlockView BlockView { get; private set; }
		public Vector3 EndPosition { get; private set; }
	}
}