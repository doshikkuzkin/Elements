using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
	public class ProjectRunner : MonoBehaviour
	{
		[Inject] private IFactory<GameRunnerController> _gameRunnerControllerFactory;
		
		private StateMachine _stateMachine = new();
		private CancellationTokenSource _gameCancellationTokenSource = new();

		private void Awake()
		{
			var gameRunnerState = _gameRunnerControllerFactory.Create();
			
			_stateMachine.Execute(gameRunnerState, _gameCancellationTokenSource.Token).Forget();
		}

		private void OnApplicationQuit()
		{
			_gameCancellationTokenSource.Cancel();
			_gameCancellationTokenSource.Dispose();
		}
	}
}