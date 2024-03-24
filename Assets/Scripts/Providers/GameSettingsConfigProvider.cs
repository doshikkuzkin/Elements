using ScriptableObjects;

namespace Providers
{
	public class GameSettingsConfigProvider : IGameSettingsConfigProvider
	{
		public GameSettingsConfigProvider(GameSettingsConfig gameSettingsConfig)
		{
			GameSettingsConfig = gameSettingsConfig;
		}

		public GameSettingsConfig GameSettingsConfig { get; }
	}
}