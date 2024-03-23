using UnityEngine;

namespace DefaultNamespace
{
	public class LevelIndexProvider : ILevelIndexProvider
	{
		private readonly IGameSettingsConfigProvider _gameSettingsConfigProvider;
		
		private int? _currentLevelIndex;

		public LevelIndexProvider(IGameSettingsConfigProvider gameSettingsConfigProvider)
		{
			_gameSettingsConfigProvider = gameSettingsConfigProvider;
		}

		public int CurrentLevelIndex => GetLevelIndex();
		public int NextLevelIndex => CurrentLevelIndex + 1 >= _gameSettingsConfigProvider.GameSettingsConfig.LevelsCount ? 0 : CurrentLevelIndex + 1;
		
		public void IncrementLevelIndex()
		{
			_currentLevelIndex = NextLevelIndex;
		}

		private int GetLevelIndex()
		{
			_currentLevelIndex??= PlayerPrefs.GetInt("LevelIndex", 0);

			return _currentLevelIndex.Value;
		}
	}
}