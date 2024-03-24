namespace Providers
{
	public interface ILevelStateProvider
	{
		bool IsLevelCleared { get; }

		void SetIsLevelCleared(bool isLevelCleared);
	}
}