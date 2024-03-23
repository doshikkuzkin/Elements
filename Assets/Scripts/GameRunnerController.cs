using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public class GameRunnerController : IGameRunnerController
	{
		private readonly ILevelController _levelController;

		public GameRunnerController(ILevelController levelController)
		{
			_levelController = levelController;
		}
		
		public UniTask Execute(CancellationToken cancellationToken)
		{
			return RunGame(cancellationToken);
		}
		
		private async UniTask RunGame(CancellationToken cancellationToken)
		{
			await _levelController.Initialize(1, cancellationToken);
			await _levelController.Execute(cancellationToken);
		}
	}
}