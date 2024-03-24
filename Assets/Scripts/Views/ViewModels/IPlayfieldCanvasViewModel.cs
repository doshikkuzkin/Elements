using System;

namespace Views.ViewModels
{
	public interface IPlayfieldCanvasViewModel : IDisposable
	{
		event Action ResetClicked;
		event Action NextClicked;

		void SetView(PlayfieldCanvasView view);
	}
}