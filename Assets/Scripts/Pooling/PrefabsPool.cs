using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
	public class PrefabsPool<T> where T : Component
	{
		private readonly GameObject _prefab;
		
		private Queue<T> _poolInstances = new();

		public PrefabsPool(GameObject prefab, int initialSize = 0)
		{
			_prefab = prefab;

			for (var i = 0; i < initialSize; i++)
			{
				CreateInstance();
			}
		}

		public T Get(Vector3 position, Transform parent = null)
		{
			if (_poolInstances.Count == 0)
			{
				CreateInstance();
			}

			return GetInstance(position, parent);
		}
		
		public void Return(T instance)
		{
			instance.gameObject.SetActive(false);
			
			_poolInstances.Enqueue(instance);
		}

		private T GetInstance(Vector3 position, Transform parent)
		{
			var instance = _poolInstances.Dequeue();

			instance.transform.position = position;
			instance.transform.SetParent(parent);
			
			instance.gameObject.SetActive(true);

			return instance;
		}
		
		private void CreateInstance()
		{
			var instance = Object.Instantiate(_prefab);
			instance.SetActive(false);
			
			_poolInstances.Enqueue(instance.GetComponent<T>());
		}
	}
}