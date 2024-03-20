using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
	public class CommandsProcessor : MonoBehaviour
	{
		private readonly Queue<ICommand> _commandsQueue = new ();
		
		public void AddCommand(ICommand command)
		{
			_commandsQueue.Enqueue(command);
		}

		private void Update()
		{
			ProcessCommands();
		}
		
		private void ProcessCommands()
		{
			while (_commandsQueue.Count > 0)
			{
				var command = _commandsQueue.Dequeue();
				
				command.Execute();
			}
		}
	}
}