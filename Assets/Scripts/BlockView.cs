using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(SortingAxis))]
	public class BlockView : MonoBehaviour
	{
		[SerializeField]
		private SortingAxis _sortingAxis;

		public CellModel CellModel { get; private set; }

		public void SetCellModel(CellModel cellModel)
		{
			CellModel = cellModel;
		}
		
		public void MoveBlock(Vector3 startPosition, Vector2 direction, float cellSize)
		{
			var newPosition = startPosition + new Vector3(direction.x * cellSize, direction.y * cellSize, 0);
			transform.localPosition = newPosition;
			
			_sortingAxis.UpdateSortingOrder(newPosition);
		}
	}
}