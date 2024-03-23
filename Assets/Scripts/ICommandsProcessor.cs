using System.Threading;

namespace DefaultNamespace
{
	public interface ICommandsProcessor
	{
		void AddCommand(ICommand command);
		void ProcessCommands(CancellationToken cancellationToken);
	}
}