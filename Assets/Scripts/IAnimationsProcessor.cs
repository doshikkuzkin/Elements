using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public interface IAnimationsProcessor
	{
		bool HasAnimationsInProcess { get; }

		UniTask PlayAnimationSequence(BlockMoveAnimationStep blockMoveAnimationStep,
			NormalizeGridAnimationStep[] normalizeAnimationSteps, CancellationToken cancellationToken);
	}
}