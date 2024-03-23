using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public interface ILevelController : IDisposable
	{
		UniTask Initialize(int levelIndex, CancellationToken cancellationToken);
		UniTask Execute(CancellationToken cancellationToken);
	}
}