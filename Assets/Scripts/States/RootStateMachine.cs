using Zenject;

namespace States
{
	public class RootStateMachine : StateMachine
	{
		public RootStateMachine(IFactory<LoaderStateMachineDecorator> loaderStateMachineDecorator)
		{
			Decorate(loaderStateMachineDecorator);
		}
	}
}