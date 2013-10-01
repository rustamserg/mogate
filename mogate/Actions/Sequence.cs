using System;
using System.Collections.Generic;


namespace mogate
{
	public class Sequence : IAction
	{
		private Queue<IAction> m_queue = new Queue<IAction>();

		public void Add(IAction action)
		{
			m_queue.Enqueue (action);
		}

		public bool Execute()
		{
			if (m_queue.Count == 0)
				return true;

			if (m_queue.Peek ().Execute ())
				m_queue.Dequeue ();

			return (m_queue.Count == 0);
		}
	}
}

