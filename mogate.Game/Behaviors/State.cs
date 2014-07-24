using System;

namespace mogate
{
	public class State<T> : IBehavior
	{
		public Type Behavior { get { return typeof(State<T>); } }

		public T EState;

		public State (T startState)
		{
			EState = startState;
		}
	}
}

