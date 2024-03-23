using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public interface IPlayfieldLoader
	{
		UniTask LoadPlayfield(LevelConfig levelConfig, CancellationToken cancellationToken);
		void ResetPlayfield(LevelConfig levelConfig);
	}
}