using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	public class BlocksDestroyAnimationStep : BaseAnimationStep
	{
		public IEnumerable<Vector2Int> BlocksToDestroyPositions { get; private set; }

		public BlocksDestroyAnimationStep(IEnumerable<Vector2Int> blocksToDestroyPositions)
		{
			BlocksToDestroyPositions = blocksToDestroyPositions;
		}
	}
}