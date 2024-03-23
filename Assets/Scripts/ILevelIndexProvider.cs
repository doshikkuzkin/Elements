namespace DefaultNamespace
{
	public interface ILevelIndexProvider
	{
		int CurrentLevelIndex { get; }
		int NextLevelIndex { get; }
		void IncrementLevelIndex();
	}
}