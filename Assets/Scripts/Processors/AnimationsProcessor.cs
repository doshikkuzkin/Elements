using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using Views;
using Views.ViewModels;

namespace Processors
{
	public class AnimationsProcessor : IAnimationsProcessor
	{
		private static int _animationsInProcessCount;

		private readonly IGridViewModel _gridViewModel;

		private readonly Queue<IAnimationStep> _blockMoveSteps = new ();
		private readonly Queue<IAnimationStep> _normalizeGridAnimationSteps = new ();

		public AnimationsProcessor(IGridViewModel gridViewModel)
		{
			_gridViewModel = gridViewModel;
		}

		public bool HasAnimationsInProcess => _animationsInProcessCount > 0;

		public void ClearAnimationsSequence()
		{
			_blockMoveSteps.Clear();
			_normalizeGridAnimationSteps.Clear();

			_animationsInProcessCount = 0;
		}
		
		public async UniTask PlayAnimationSequence(BlockMoveAnimationStep blockMoveAnimationStep,
			NormalizeGridAnimationStepsContainer normalizeAnimationStepsContainer, CancellationToken cancellationToken)
		{
			_animationsInProcessCount++;
			
			var blockMoveStepToWait = _blockMoveSteps.Count > 0 ? _blockMoveSteps.Last() : null;
			var normalizeStepToWait = _normalizeGridAnimationSteps.Count > 0 ? _normalizeGridAnimationSteps.Last() : null;
			
			_blockMoveSteps.Enqueue(blockMoveAnimationStep);
			_normalizeGridAnimationSteps.Enqueue(normalizeAnimationStepsContainer);

			var disabledBlocks = DisableMovingBlocks(blockMoveAnimationStep, normalizeAnimationStepsContainer.NormalizeGridAnimationSteps);

			if (blockMoveStepToWait != null)
			{
				await UniTask.WaitUntil(() => blockMoveStepToWait.IsCompleted, cancellationToken: cancellationToken);
			}
			
			await PlaySwapAnimation(blockMoveAnimationStep, cancellationToken);
			
			blockMoveAnimationStep.SetIsCompleted(true);
			_blockMoveSteps.Dequeue();

			if (normalizeStepToWait != null)
			{
				await UniTask.WaitUntil(() => normalizeStepToWait.IsCompleted, cancellationToken: cancellationToken);
			}

			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				await PlayNormalizeAnimation(normalizeAnimationStepsContainer.NormalizeGridAnimationSteps, cancellationToken);
				
				foreach (var disabledBlock in disabledBlocks)
				{
					disabledBlock.SetIsAllowedToMove(true);
				}
				
				normalizeAnimationStepsContainer.SetIsCompleted(true);
				_normalizeGridAnimationSteps.Dequeue();
			}
			finally
			{
				_animationsInProcessCount--;
			}
		}

		private async UniTask PlayNormalizeAnimation(NormalizeGridAnimationStep[] normalizeAnimationSteps,
			CancellationToken cancellationToken)
		{
			foreach (var normalizeAnimationStep in normalizeAnimationSteps)
			{
				if (normalizeAnimationStep.MoveAnimationSteps != null)
				{
					var moveAnimationsList = normalizeAnimationStep.MoveAnimationSteps.Select(moveAnimationStep =>
						PlaySwapAnimation(moveAnimationStep, cancellationToken));

					await UniTask.WhenAll(moveAnimationsList);
				}

				cancellationToken.ThrowIfCancellationRequested();

				var blocksToDestroyPositions =
					normalizeAnimationStep.BlocksDestroyAnimationStep?.BlocksToDestroyPositions?.ToArray();

				if (blocksToDestroyPositions != null)
				{
					await blocksToDestroyPositions
						.Select(cellPosition =>
						{
							if (_gridViewModel.TryGetBlockView(cellPosition, out var blockView))
							{
								return blockView.DestroyBlock(cancellationToken);
							}

							return UniTask.CompletedTask;
						});

					_gridViewModel.DestroyCellsViews(blocksToDestroyPositions);
				}
			}
		}

		private async UniTask PlaySwapAnimation(BlockMoveAnimationStep blockMoveAnimationStep,
			CancellationToken cancellationToken)
		{
			var blockViews = blockMoveAnimationStep.BlockMoveInfo
				.Select(blockMoveInfo =>
					_gridViewModel.TryGetBlockView(blockMoveInfo.StartBlockPosition, out var blockView)
						? blockView
						: null).ToArray();

			var firstCellToSwap = blockMoveAnimationStep.BlockMoveInfo[0].StartBlockPosition;
			var secondCellToSwap = firstCellToSwap + blockMoveAnimationStep.BlockMoveInfo[0].Direction;

			_gridViewModel.SwapCellsViews(firstCellToSwap, secondCellToSwap);

			await UniTask.WhenAll(blockMoveAnimationStep.BlockMoveInfo.Select((blockMoveInfo, index) =>
			{
				if (blockViews[index] == null)
				{
					return UniTask.CompletedTask;
				}
				
				var newPosition = blockViews[index].transform.localPosition + new Vector3(
					blockMoveInfo.Direction.x * _gridViewModel.CellSize,
					blockMoveInfo.Direction.y * _gridViewModel.CellSize,
					0);

				return blockViews[index].MoveBlock(newPosition, cancellationToken);
			}));
		}

		private List<BlockView> DisableMovingBlocks(BlockMoveAnimationStep blockMoveAnimationStep,
			IEnumerable<NormalizeGridAnimationStep> normalizeAnimationSteps)
		{
			var animatedCells = new HashSet<Vector2Int>();

			foreach (var blockMoveInfo in blockMoveAnimationStep.BlockMoveInfo)
			{
				animatedCells.Add(blockMoveInfo.StartBlockPosition);
			}

			foreach (var normalizeAnimationStep in normalizeAnimationSteps)
			{
				if (normalizeAnimationStep.MoveAnimationSteps != null)
				{
					foreach (var moveAnimationStep in normalizeAnimationStep.MoveAnimationSteps)
					{
						foreach (var blockMoveInfo in moveAnimationStep.BlockMoveInfo)
						{
							animatedCells.Add(blockMoveInfo.StartBlockPosition);
						}
					}
				}

				var cellsToDestroy = normalizeAnimationStep.BlocksDestroyAnimationStep?.BlocksToDestroyPositions;

				if (cellsToDestroy != null)
				{
					foreach (var cell in cellsToDestroy)
					{
						animatedCells.Add(cell);
					}
				}
			}

			var disabledBlocks = new List<BlockView>();

			foreach (var cell in animatedCells)
			{
				if (_gridViewModel.TryGetBlockView(cell, out var blockView))
				{
					blockView.SetIsAllowedToMove(false);
					disabledBlocks.Add(blockView);
				}
			}

			return disabledBlocks;
		}
	}
}