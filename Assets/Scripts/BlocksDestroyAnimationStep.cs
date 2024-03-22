using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
	public struct BlocksDestroyAnimationStep
	{
		public IEnumerable<Vector2Int> BlocksToDestroyPositions { get; private set; }

		public BlocksDestroyAnimationStep(IEnumerable<Vector2Int> blocksToDestroyPositions)
		{
			BlocksToDestroyPositions = blocksToDestroyPositions;
		}
	}
}