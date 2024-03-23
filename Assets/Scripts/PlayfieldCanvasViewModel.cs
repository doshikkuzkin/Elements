using System;

namespace DefaultNamespace
{
	public class PlayfieldCanvasViewModel : IPlayfieldCanvasViewModel, IDisposable
	{
		private PlayfieldCanvasView _view;
		
		public event Action ResetClicked;
		
		public void SetView(PlayfieldCanvasView view)
		{
			_view = view;

			_view.ResetClicked += OnResetClicked;
		}

		public void Dispose()
		{
			_view.ResetClicked -= OnResetClicked;
		}
		
		private void OnResetClicked()
		{
			ResetClicked?.Invoke();
		}
	}
}