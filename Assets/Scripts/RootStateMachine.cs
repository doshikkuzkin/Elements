using Zenject;

namespace DefaultNamespace
{
	public class RootStateMachine : StateMachine
	{
		public RootStateMachine(IFactory<LoaderStateMachineDecorator> loaderStateMachineDecorator)
		{
			Decorate(loaderStateMachineDecorator);
		}
	}
}