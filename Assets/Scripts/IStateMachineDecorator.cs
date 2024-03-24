using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
	public interface IStateMachineDecorator
	{
		UniTask OnBeforeInitialize();
		UniTask OnBeforeExecute();
		UniTask OnBeforeStop();
	}
}