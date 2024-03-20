using UnityEngine;
using UnityEngine.Rendering;

namespace DefaultNamespace
{
	[RequireComponent(typeof(SortingGroup))]
	public class SortingAxis : MonoBehaviour
	{
		[SerializeField] private SortingGroup _sortingGroup;
		
		private void OnEnable()
		{
			UpdateSortingOrder();
		}
		
		private void UpdateSortingOrder()
		{
			var position = transform.position;
			
			_sortingGroup.sortingOrder = Mathf.CeilToInt(position.x + position.y);
		}
	}
}