using System;

namespace Observers
{
	public class LevelWinObserver : ILevelWinObserver
	{
		public event Action LevelWin;

		public void RequestLevelWin()
		{
			LevelWin?.Invoke();
		}
	}
}