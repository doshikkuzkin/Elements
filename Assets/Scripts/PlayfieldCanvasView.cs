using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
	public class PlayfieldCanvasView : MonoBehaviour
	{
		[SerializeField] private Canvas _canvas;
		[SerializeField] private Button _resetButton;
		
		public event Action ResetClicked;

		private void Awake()
		{
			_canvas.worldCamera = Camera.main;
			
			_resetButton.onClick.AddListener(OnResetClicked);
		}

		private void OnResetClicked()
		{
			ResetClicked?.Invoke();
		}
	}
}