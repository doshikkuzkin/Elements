namespace Providers
{
	public class LevelStateProvider : ILevelStateProvider
	{
		public bool IsLevelCleared { get; private set; }

		public void SetIsLevelCleared(bool isLevelCleared)
		{
			IsLevelCleared = isLevelCleared;
		}
	}
}