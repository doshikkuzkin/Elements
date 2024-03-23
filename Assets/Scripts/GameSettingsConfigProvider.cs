namespace DefaultNamespace
{
	public class GameSettingsConfigProvider : IGameSettingsConfigProvider
	{
		private readonly GameSettingsConfig _gameSettingsConfig;

		public GameSettingsConfigProvider(GameSettingsConfig gameSettingsConfig)
		{
			_gameSettingsConfig = gameSettingsConfig;
		}
		public GameSettingsConfig GameSettingsConfig => _gameSettingsConfig;
	}
}