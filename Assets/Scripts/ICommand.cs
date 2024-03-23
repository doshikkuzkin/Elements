using System.Threading;

namespace DefaultNamespace
{
	public interface ICommand
	{
		void Execute(CancellationToken cancellationToken);
	}
}