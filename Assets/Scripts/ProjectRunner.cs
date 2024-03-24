using System.Threading;
using Controllers;
using Cysharp.Threading.Tasks;
using States;
using UnityEngine;
using Zenject;

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