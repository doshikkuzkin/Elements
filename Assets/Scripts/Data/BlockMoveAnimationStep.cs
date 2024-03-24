namespace Data
{
	public class BlockMoveAnimationStep : BaseAnimationStep
	{
		public BlockMoveInfo[] BlockMoveInfo { get; private set; }

		public BlockMoveAnimationStep(params BlockMoveInfo[] blockMoveInfo)
		{
			BlockMoveInfo = blockMoveInfo;
		}
	}
}