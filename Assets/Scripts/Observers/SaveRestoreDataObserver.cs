using System;

namespace Observers
{
	public class SaveRestoreDataObserver : ISaveRestoreDataObserver
	{
		public event Action SaveRequested;
		public event Action ClearRequested;

		public void RequestSave()
		{
			SaveRequested?.Invoke();
		}

		public void RequestClear()
		{
			ClearRequested?.Invoke();
		}
	}
}