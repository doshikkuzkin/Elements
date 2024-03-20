using UnityEngine;

namespace DefaultNamespace
{
	public class DebugScript : MonoBehaviour
	{
		[SerializeField] private GridSpawner _gridSpawner;
		[SerializeField] private LevelConfig _levelConfig;

		private void OnEnable()
		{
			_gridSpawner.SpawnGrid((GridModel) _levelConfig.GridModel.Clone());
		}
	}
}