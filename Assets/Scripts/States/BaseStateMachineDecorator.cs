using Cysharp.Threading.Tasks;

namespace States
{
	public class BaseStateMachineDecorator : IStateMachineDecorator
	{
		public virtual UniTask OnBeforeInitialize()
		{
			return UniTask.CompletedTask;
		}

		public virtual UniTask OnBeforeExecute()
		{
			return UniTask.CompletedTask;
		}

		public virtual UniTask OnBeforeStop()
		{
			return UniTask.CompletedTask;
		}
	}
}