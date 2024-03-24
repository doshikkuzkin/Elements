using System;

namespace DefaultNamespace
{
	public interface IResetPlayfieldNotifier
	{
		event Action PlayfieldReset;

		void NotifyPlayfieldReset();
	}
}