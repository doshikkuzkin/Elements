using System;

namespace Observers
{
	public interface ILevelWinObserver
	{
		event Action LevelWin;

		void RequestLevelWin();
	}
}