using UnityEngine;
using UnityEngine.Rendering;

namespace Components
{
	[RequireComponent(typeof(SortingGroup))]
	public class SortingAxis : MonoBehaviour
	{
		[SerializeField] private SortingGroup _sortingGroup;
		
		private void OnEnable()
		{
			UpdateSortingOrder(transform.localPosition);
		}
		
		public void UpdateSortingOrder(Vector3 position)
		{
			_sortingGroup.sortingOrder = Mathf.CeilToInt(position.x + position.y);
		}
	}
}