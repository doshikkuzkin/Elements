using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
	public class BlockView : MonoBehaviour
	{
		private void OnMouseUpAsButton()
		{
			Debug.Log($"Clicked {gameObject.name} object");
		}
	}
}