using UnityEngine;

namespace DefaultNamespace
{
	[CreateAssetMenu(fileName = "GameSettingsConfig", menuName = "Configs/GameSettings", order = 0)]
	public class GameSettingsConfig : ScriptableObject
	{
		[SerializeField] private int _levelsCount;

		public int LevelsCount => _levelsCount;
	}
}