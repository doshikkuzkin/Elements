using System.Threading;

namespace Commands
{
	public interface ICommand
	{
		void Execute(CancellationToken cancellationToken);
	}
}