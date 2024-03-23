using System;

namespace DefaultNamespace
{
	public interface IPlayfieldCanvasViewModel : IDisposable
	{
		event Action ResetClicked;
		event Action NextClicked;
		
		void SetView(PlayfieldCanvasView view);
	}
}