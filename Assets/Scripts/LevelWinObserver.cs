using System;

namespace DefaultNamespace
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