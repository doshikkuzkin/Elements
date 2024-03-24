using Cysharp.Threading.Tasks;

namespace States
{
	public interface IStateMachineDecorator
	{
		UniTask OnBeforeInitialize();
		UniTask OnBeforeExecute();
		UniTask OnBeforeStop();
	}
}