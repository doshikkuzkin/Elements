using System.Collections.Generic;

namespace Data
{
	public class NormalizeGridAnimationStep : BaseAnimationStep
	{
		public IEnumerable<BlockMoveAnimationStep> MoveAnimationSteps { get; private set; }
		public BlocksDestroyAnimationStep BlocksDestroyAnimationStep { get; private set; }

		public NormalizeGridAnimationStep(IEnumerable<BlockMoveAnimationStep> moveAnimationSteps,
			BlocksDestroyAnimationStep blocksDestroyAnimationStep)
		{
			MoveAnimationSteps = moveAnimationSteps;
			BlocksDestroyAnimationStep = blocksDestroyAnimationStep;
		}
	}
}