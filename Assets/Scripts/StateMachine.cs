using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public class StateMachine
	{
		private State CurrentState { get; set; }

		public async UniTask Execute(State state, CancellationToken cancellationToken)
		{
			CurrentState = state;

			var stateResult = await RunCurrentState(cancellationToken);

			if (stateResult.NextState != null)
			{
				await Execute(stateResult.NextState, cancellationToken);
			}
		}
		
		private async UniTask<StateResult> RunCurrentState(CancellationToken cancellationToken)
		{
			await CurrentState.Initialize(cancellationToken);
			var stateResult = await CurrentState.Execute(cancellationToken);
			await CurrentState.Stop(cancellationToken);

			return stateResult;
		}
	}
}