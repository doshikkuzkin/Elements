using System;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
	public class PlayfieldCanvasView : MonoBehaviour
	{
		[SerializeField] private Canvas _canvas;
		[SerializeField] private Button _resetButton;
		[SerializeField] private Button _nextButton;

		private void Awake()
		{
			_canvas.worldCamera = Camera.main;

			_resetButton.onClick.AddListener(OnResetClicked);
			_nextButton.onClick.AddListener(OnNextClicked);
		}

		public event Action ResetClicked;
		public event Action NextClicked;

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