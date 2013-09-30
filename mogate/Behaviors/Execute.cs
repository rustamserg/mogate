using System;
using System.Collections.Generic;


namespace mogate
{
	public interface IAction
	{
		bool Execute();
	};

	public class Execute : IBehavior
	{
		public Type Behavior { get { return typeof(Execute); } }

		private Queue<IAction> m_actions = new Queue<IAction>();

		public void Start(IAction action)
		{
			m_actions.Enqueue(action);
		}

		public void Update()
		{
			if (m_actions.Count == 0)
				return;

			if (m_actions.Peek ().Execute ())
				m_actions.Dequeue ();
		}
	}
}

