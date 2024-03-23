using System.Threading;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public interface IAddressableAssetsLoader
	{
		UniTask<T> LoadAsset<T>(string key, CancellationToken cancellationToken);
		void UnloadAssets();
	}
}