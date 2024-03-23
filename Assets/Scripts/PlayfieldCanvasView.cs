using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
	public class PlayfieldCanvasView : MonoBehaviour
	{
		[SerializeField] private Canvas _canvas;
		[SerializeField] private Button _resetButton;
		[SerializeField] private Button _nextButton;
		
		public event Action ResetClicked;
		public event Action NextClicked;

		private void Awake()
		{
			_canvas.worldCamera = Camera.main;
			
			_resetButton.onClick.AddListener(OnResetClicked);
			_nextButton.onClick.AddListener(OnNextClicked);
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