namespace Views
{
	public interface ILoaderView
	{
		bool IsLoaderShown { get; }
		bool IsLoaderHidden { get; }
		
		void ShowLoader();
		void HideLoader();
	}
}