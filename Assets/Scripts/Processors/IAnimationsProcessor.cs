using System.Threading;
using Cysharp.Threading.Tasks;
using Data;

namespace Processors
{
	public interface IAnimationsProcessor
	{
		bool HasAnimationsInProcess { get; }

		void ClearAnimationsSequence();
		
		UniTask PlayAnimationSequence(BlockMoveAnimationStep blockMoveAnimationStep,
			NormalizeGridAnimationStepsContainer normalizeAnimationSteps, CancellationToken cancellationToken);
	}
}