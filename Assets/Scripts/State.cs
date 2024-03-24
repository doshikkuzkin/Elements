using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public abstract class State
	{
		public UniTaskCompletionSource<StateResult> StateCompletionSource { get; } = new();

		public abstract UniTask Initialize(CancellationToken cancellationToken);
		public abstract UniTask<StateResult> Execute(CancellationToken cancellationToken);
		public abstract UniTask Stop(CancellationToken cancellationToken);
	}
}