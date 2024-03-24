namespace Data
{
	public interface IAnimationStep
	{
		bool IsCompleted { get; }
		void SetIsCompleted(bool isCompleted);
	}
}