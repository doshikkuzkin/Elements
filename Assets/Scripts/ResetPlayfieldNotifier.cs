using System;

namespace DefaultNamespace
{
	public class ResetPlayfieldNotifier : IResetPlayfieldNotifier
	{
		public event Action PlayfieldReset;
		
		public void NotifyPlayfieldReset()
		{
			PlayfieldReset?.Invoke();
		}
	}
}