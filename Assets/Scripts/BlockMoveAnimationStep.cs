namespace DefaultNamespace
{
	public struct BlockMoveAnimationStep
	{
		public BlockMoveInfo[] BlockMoveInfo { get; private set; }

		public BlockMoveAnimationStep(params BlockMoveInfo[] blockMoveInfo)
		{
			BlockMoveInfo = blockMoveInfo;
		}
	}
}