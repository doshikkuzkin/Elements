using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;

namespace DefaultNamespace
{
	public class StateMachine
	{
		private readonly List<IStateMachineDecorator> _decorators = new ();
		private State CurrentState { get; set; }

		protected void Decorate<T>(IFactory<T> decoratorFactory) where T : IStateMachineDecorator
		{
			_decorators.Add(decoratorFactory.Create());
		}

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
			foreach (var decorator in _decorators)
			{
				await decorator.OnBeforeInitialize();
			}
			
			await CurrentState.Initialize(cancellationToken);

			foreach (var decorator in _decorators)
			{
				await decorator.OnBeforeExecute();
			}
			
			var stateResult = await CurrentState.Execute(cancellationToken);
			
			foreach (var decorator in _decorators)
			{
				await decorator.OnBeforeStop();
			}
			
			await CurrentState.Stop(cancellationToken);

			return stateResult;
		}
	}
}