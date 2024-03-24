using Data;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "GameSettingsConfig", menuName = "Configs/GameSettings", order = 0)]
	public class GameSettingsConfig : ScriptableObject
	{
		[SerializeField] private int _levelsCount;
		[SerializeField] private BlockTypeData[] _blockTypesData;

		public int LevelsCount => _levelsCount;
		public BlockTypeData[] BlockTypesData => _blockTypesData;
	}
}