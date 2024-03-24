using System.Threading;
using Cysharp.Threading.Tasks;

namespace States
{
	public class StateBase : State
	{
		public override UniTask Initialize(CancellationToken cancellationToken)
		{
			return UniTask.CompletedTask;
		}

		public override UniTask<StateResult> Execute(CancellationToken cancellationToken)
		{
			return StateCompletionSource.Task.AttachExternalCancellation(cancellationToken);
		}

		public override UniTask Stop(CancellationToken cancellationToken)
		{
			return UniTask.CompletedTask;
		}
	}
}