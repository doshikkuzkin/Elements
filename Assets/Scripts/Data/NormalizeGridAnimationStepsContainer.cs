namespace Data
{
	public class NormalizeGridAnimationStepsContainer : BaseAnimationStep
	{
		public NormalizeGridAnimationStep[] NormalizeGridAnimationSteps { get; private set; }

		public NormalizeGridAnimationStepsContainer(NormalizeGridAnimationStep[] normalizeGridAnimationSteps)
		{
			NormalizeGridAnimationSteps = normalizeGridAnimationSteps;
		}
	}
}