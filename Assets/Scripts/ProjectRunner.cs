using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
	public class ProjectRunner : MonoBehaviour
	{
		[Inject] private IGameRunnerController _gameRunnerController;
		
		private CancellationTokenSource _gameCancellationTokenSource = new();

		private void Awake()
		{
			_gameRunnerController.Execute(_gameCancellationTokenSource.Token).Forget();
		}

		private void OnDestroy()
		{
			_gameCancellationTokenSource.Cancel();
			_gameCancellationTokenSource.Dispose();
		}
	}
}