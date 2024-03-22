using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
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

		public void SetCellModel(CellModel cellModel)
		{
			CellModel = cellModel;
		}

		public void SetIsAllowedToMove(bool isAllowedToMove)
		{
			IsAllowedToMove = isAllowedToMove;
		}
		
		public async UniTask MoveBlock(Vector3 newPosition)
		{
			await transform.DOLocalMove(newPosition, AnimationDuration).OnUpdate(UpdateSortingOrder).ToUniTask();
		}

		public UniTask DestroyBlock()
		{
			_destroyCompletionSource = new UniTaskCompletionSource();
			_animator.SetTrigger(DestroyAnimationTriggerName);

			return _destroyCompletionSource.Task;
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