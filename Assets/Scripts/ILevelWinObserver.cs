using System;

namespace DefaultNamespace
{
	public interface ILevelWinObserver
	{
		event Action LevelWin;
		
		void RequestLevelWin();
	}
}