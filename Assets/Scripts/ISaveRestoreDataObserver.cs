using System;

namespace DefaultNamespace
{
	public interface ISaveRestoreDataObserver
	{
		event Action SaveRequested;
		event Action ClearRequested;

		void RequestSave();
		void RequestClear();
	}
}