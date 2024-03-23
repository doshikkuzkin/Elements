using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public interface IGameRunnerController
	{
		UniTask Execute(CancellationToken cancellationToken);
	}
}