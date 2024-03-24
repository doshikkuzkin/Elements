namespace States
{
	public class StateResult
	{
		public StateResult(State nextState)
		{
			NextState = nextState;
		}

		public State NextState { get; }
	}
}