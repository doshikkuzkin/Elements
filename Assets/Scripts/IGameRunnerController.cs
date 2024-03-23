using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public interface IGameRunnerController : IDisposable
	{
		UniTask Execute(CancellationToken cancellationToken);
	}
}