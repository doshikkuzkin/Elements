using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public interface IPlayfieldLoader : IDisposable
	{
		UniTask LoadPlayfield(LevelConfig levelConfig, CancellationToken cancellationToken);
		void ResetPlayfield(LevelConfig levelConfig);
	}
}