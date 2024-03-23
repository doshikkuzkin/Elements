using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public interface ILevelController
	{
		UniTask Initialize(int levelIndex, CancellationToken cancellationToken);
		UniTask Execute(CancellationToken cancellationToken);
	}
}