using System.Threading;
using Cysharp.Threading.Tasks;

namespace Assressables
{
	public interface IAddressableAssetsLoader
	{
		UniTask<T> LoadAsset<T>(string key, CancellationToken cancellationToken);
		void UnloadAssets();
	}
}