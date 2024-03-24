using System;

namespace Observers
{
	public class ResetPlayfieldObserver : IResetPlayfieldObserver
	{
		public event Action PlayfieldReset;

		public void NotifyPlayfieldReset()
		{
			PlayfieldReset?.Invoke();
		}
	}
}