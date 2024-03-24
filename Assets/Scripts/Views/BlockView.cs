using System.Threading;
using Components;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using UnityEngine;

namespace Views
{
	[RequireComponent(typeof(SortingAxis))]
	public class BlockView : MonoBehaviour
	{
		private const string DestroyAnimationTriggerName = "Destroy";
		private const float AnimationDuration = .5f;
		
		[SerializeField]
		private SortingAxis _sortingAxis;

		[SerializeField] private Animator _animator;
		
		private UniTaskCompletionSource _destroyCompletionSource;

		public CellModel CellModel { get; private set; }
		public bool IsAllowedToMove { get; private set; } = true;
		public int BlockType { get; private set; }

		public void SetCellModel(CellModel cellModel)
		{
			CellModel = cellModel;
		}

		public void SetIsAllowedToMove(bool isAllowedToMove)
		{
			IsAllowedToMove = isAllowedToMove;
		}

		public void SetBlockType(int blockType)
		{
			BlockType = blockType;
		}
		
		public async UniTask MoveBlock(Vector3 newPosition, CancellationToken cancellationToken)
		{
			await transform.DOLocalMove(newPosition, AnimationDuration).OnUpdate(UpdateSortingOrder).WithCancellation(cancellationToken);
		}

		public UniTask DestroyBlock(CancellationToken cancellationToken)
		{
			_destroyCompletionSource = new UniTaskCompletionSource();
			_animator.SetTrigger(DestroyAnimationTriggerName);

			return _destroyCompletionSource.Task.AttachExternalCancellation(cancellationToken);
		}
		
		private void UpdateSortingOrder()
		{
			_sortingAxis.UpdateSortingOrder(transform.localPosition);
		}

		private void OnDestroyBlock()
		{
			_destroyCompletionSource.TrySetResult();
		}
	}
}