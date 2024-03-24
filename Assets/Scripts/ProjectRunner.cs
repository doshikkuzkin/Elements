using System.Threading;
using Controllers;
using Cysharp.Threading.Tasks;
using States;
using UnityEngine;
using Zenject;

public class ProjectRunner : MonoBehaviour
{
	private readonly CancellationTokenSource _gameCancellationTokenSource = new();

	[Inject] private IFactory<PlayfieldState> _gameRunnerControllerFactory;
	[Inject] private IFactory<RootStateMachine> _rootStateMachineFactory;

	private StateMachine _stateMachine;

	private void Awake()
	{
		DisableAutoRotation();
		
		var gameRunnerState = _gameRunnerControllerFactory.Create();

		_stateMachine = _rootStateMachineFactory.Create();
		_stateMachine.Execute(gameRunnerState, _gameCancellationTokenSource.Token).Forget();
	}
	
	private void DisableAutoRotation()
	{
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.orientation = ScreenOrientation.Portrait;
	}

	private void OnApplicationQuit()
	{
		_gameCancellationTokenSource.Cancel();
		_gameCancellationTokenSource.Dispose();
	}
}