namespace DefaultNamespace
{
	public class StateResult
	{
		public State NextState { get; }

		public StateResult(State nextState)
		{
			NextState = nextState;
		}
	}
}