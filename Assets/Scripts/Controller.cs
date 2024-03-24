using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public abstract class Controller
	{
		public abstract UniTask Initialize(CancellationToken cancellationToken);
		public abstract UniTask Execute(CancellationToken cancellationToken);
		public abstract UniTask Stop(CancellationToken cancellationToken);
	}
}