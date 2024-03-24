using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
	public class ProjectRunner : MonoBehaviour
	{
		[Inject] private IFactory<GameRunnerController> _gameRunnerControllerFactory;
		[Inject] private IFactory<RootStateMachine> _rootStateMachineFactory;
		
		private StateMachine _stateMachine;
		private CancellationTokenSource _gameCancellationTokenSource = new();

		private void Awake()
		{
			var gameRunnerState = _gameRunnerControllerFactory.Create();

			_stateMachine = _rootStateMachineFactory.Create();
			_stateMachine.Execute(gameRunnerState, _gameCancellationTokenSource.Token).Forget();
		}

		private void OnApplicationQuit()
		{
			_gameCancellationTokenSource.Cancel();
			_gameCancellationTokenSource.Dispose();
		}
	}
}