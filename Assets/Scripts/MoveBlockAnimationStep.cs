using UnityEngine;

namespace DefaultNamespace
{
	public class MoveBlockAnimationStep
	{
		public BlockView BlockView { get; private set; }
		public Vector3 EndPosition { get; private set; }

		public MoveBlockAnimationStep(BlockView blockView, Vector3 endPosition)
		{
			BlockView = blockView;
			EndPosition = endPosition;
		}
	}
}