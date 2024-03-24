namespace States
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