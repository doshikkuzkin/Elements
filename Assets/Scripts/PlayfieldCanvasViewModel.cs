using System;

namespace DefaultNamespace
{
	public class PlayfieldCanvasViewModel : IPlayfieldCanvasViewModel
	{
		private PlayfieldCanvasView _view;
		
		public event Action ResetClicked;
		public event Action NextClicked;
		
		public void SetView(PlayfieldCanvasView view)
		{
			_view = view;

			_view.ResetClicked += OnResetClicked;
			_view.NextClicked += OnNextClicked;
		}

		public void Dispose()
		{
			_view.ResetClicked -= OnResetClicked;
		}
		
		private void OnResetClicked()
		{
			ResetClicked?.Invoke();
		}
		
		private void OnNextClicked()
		{
			NextClicked?.Invoke();
		}
	}
}