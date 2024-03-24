using UnityEngine;

namespace DefaultNamespace
{
	public class LoaderView : MonoBehaviour, ILoaderView
	{
		private const string ShowAnimationTriggerName = "Show";
		private const string HideAnimationTriggerName = "Hide";
		
		[SerializeField] private Animator _animator;

		public bool IsLoaderShown { get; private set; } = true;
		public bool IsLoaderHidden { get; private set; }

		public void ShowLoader()
		{
			if (IsLoaderShown)
			{
				return;
			}
			
			IsLoaderHidden = false;
			
			_animator.SetTrigger(ShowAnimationTriggerName);
		}
		
		public void HideLoader()
		{
			if (IsLoaderHidden)
			{
				return;
			}
			
			IsLoaderShown = false;
			
			_animator.SetTrigger(HideAnimationTriggerName);
		}

		private void OnLoaderShown()
		{
			IsLoaderShown = true;
		}

		private void OnLoaderHidden()
		{
			IsLoaderHidden = true;
		}
	}
}