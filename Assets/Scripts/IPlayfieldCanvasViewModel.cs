using System;

namespace DefaultNamespace
{
	public interface IPlayfieldCanvasViewModel
	{
		event Action ResetClicked;
		
		void SetView(PlayfieldCanvasView view);
	}
}