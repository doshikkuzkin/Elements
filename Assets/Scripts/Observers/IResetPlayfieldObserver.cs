using System;

namespace Observers
{
	public interface IResetPlayfieldObserver
	{
		event Action PlayfieldReset;

		void NotifyPlayfieldReset();
	}
}