using System.Threading;
using Commands;

namespace Processors
{
	public interface ICommandsProcessor
	{
		void AddCommand(ICommand command);
		void ProcessCommands(CancellationToken cancellationToken);
	}
}