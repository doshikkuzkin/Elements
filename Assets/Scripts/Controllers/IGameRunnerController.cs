using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Controllers
{
	public interface IGameRunnerController : IDisposable
	{
		UniTask Execute(CancellationToken cancellationToken);
	}
}