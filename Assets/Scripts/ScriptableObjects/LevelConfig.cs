using Data;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/Level", order = 0)]
	public class LevelConfig : ScriptableObject
	{
		public GridModel GridModel;
	}
}