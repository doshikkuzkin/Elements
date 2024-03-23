using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
	public class ProjectRunner : MonoBehaviour
	{
		[Inject] private IGameRunnerController _gameRunnerController;
		
		[SerializeField] private GameSettingsConfig _gameSettingsConfig;
		
		private CancellationTokenSource _gameCancellationTokenSource = new();

		private void Awake()
		{
			_gameRunnerController.Execute(_gameSettingsConfig, _gameCancellationTokenSource.Token).Forget();
		}

		private void OnApplicationQuit()
		{
			_gameCancellationTokenSource.Cancel();
			_gameCancellationTokenSource.Dispose();

			_gameRunnerController.Dispose();
		}
	}
}