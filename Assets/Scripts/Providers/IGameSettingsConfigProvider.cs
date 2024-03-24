using ScriptableObjects;

namespace Providers
{
	public interface IGameSettingsConfigProvider
	{
		GameSettingsConfig GameSettingsConfig { get; }
	}
}