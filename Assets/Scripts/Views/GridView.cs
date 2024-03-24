using UnityEngine;

namespace Views
{
	public class GridView : MonoBehaviour
	{
		[SerializeField] private Transform _gridPivot;
		[SerializeField] private Transform _gridParent;
		[SerializeField] public float _gridCellSize;
		[SerializeField] private float _maxGridWidth;

		public Transform GridPivot => _gridPivot;
		public Transform GridParent => _gridParent;
		public float GridCellSize => _gridCellSize;
		public float MaxGridWidth => _maxGridWidth;
	}
}