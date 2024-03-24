using System.Collections.Generic;
using System.Threading;
using Commands;

namespace Processors
{
	public class CommandsProcessor : ICommandsProcessor
	{
		private readonly Queue<ICommand> _commandsQueue = new();

		public void AddCommand(ICommand command)
		{
			_commandsQueue.Enqueue(command);
		}

		public void ProcessCommands(CancellationToken cancellationToken)
		{
			while (_commandsQueue.Count > 0)
			{
				var command = _commandsQueue.Dequeue();

				command.Execute(cancellationToken);
			}
		}
	}
}