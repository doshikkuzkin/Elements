using Cysharp.Threading.Tasks;

namespace States
{
	public class BaseStateMachineDecorator : IStateMachineDecorator
	{
		public virtual UniTask OnBeforeInitialize() => UniTask.CompletedTask;
		public virtual UniTask OnBeforeExecute() => UniTask.CompletedTask;
		public virtual UniTask OnBeforeStop() => UniTask.CompletedTask;
	}
}