using System;
using System.Collections.Generic;


namespace mogate
{
	public interface IAction
	{
		bool Execute();
	};

	public class ActionQueue : IBehavior
	{
		public Type Behavior { get { return typeof(ActionQueue); } }

		private Queue<IAction> m_actions = new Queue<IAction>();

		public void Add(IAction action)
		{
			m_actions.Enqueue (action);
		}

		public void Update()
		{
			if (m_actions.Count == 0)
				return;

			var action = m_actions.Peek ();
			if (action.Execute ())
				m_actions.Dequeue ();
		}
	}
}

