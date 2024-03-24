using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assressables
{
	public class AddressableAssetsLoader : IAddressableAssetsLoader
	{
		private readonly List<AsyncOperationHandle> _loadHandles = new ();
		
		public UniTask<T> LoadAsset<T>(string key, CancellationToken cancellationToken)
		{
			var asyncOperationHandle = Addressables.LoadAssetAsync<T>(key);
			_loadHandles.Add(asyncOperationHandle);

			return asyncOperationHandle.WithCancellation(cancellationToken);
		}

		public void UnloadAssets()
		{
			foreach (var loadHandle in _loadHandles)
			{
				Addressables.Release(loadHandle);
			}
			
			_loadHandles.Clear();
		}
	}
}