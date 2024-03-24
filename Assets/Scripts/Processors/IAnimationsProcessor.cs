using System.Threading;
using Cysharp.Threading.Tasks;
using Data;

namespace Processors
{
	public interface IAnimationsProcessor
	{
		bool HasAnimationsInProcess { get; }

		UniTask PlayAnimationSequence(BlockMoveAnimationStep blockMoveAnimationStep,
			NormalizeGridAnimationStep[] normalizeAnimationSteps, CancellationToken cancellationToken);
	}
}