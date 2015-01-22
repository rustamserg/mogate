using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Elizabeth
{
	public class Sequence : IAction
	{
		private Queue<IAction> m_queue = new Queue<IAction>();

		public void Add(IAction action)
		{
			m_queue.Enqueue (action);
		}

		public bool Execute(GameTime gameTime)
		{
			if (m_queue.Count == 0)
				return true;

			if (m_queue.Peek ().Execute (gameTime)) {
				m_queue.Dequeue ();
				Execute (gameTime);
			}
			return (m_queue.Count == 0);
		}
	}
}

