using System;
using System.Collections.Generic;


namespace mogate
{
	public interface IAction
	{
		bool Execute();
	};

	public class ActionList : IAspect
	{
		public Type Behavior { get { return typeof(ActionList); } }

		private Queue<IAction> m_actions = new Queue<IAction>();

		public void Push(IAction action)
		{
			m_actions.Enqueue (action);
		}

		public void Update()
		{
			if (m_actions.Count == 0)
				return;

			var action = m_actions.Dequeue ();
			if (!action.Execute ())
				m_actions.Enqueue (action);
		}
	}
}

