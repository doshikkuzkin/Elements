namespace Data
{
	public class BaseAnimationStep : IAnimationStep
	{
		public bool IsCompleted { get; private set; }
		
		public void SetIsCompleted(bool isCompleted)
		{
			IsCompleted = isCompleted;
		}
	}
}