using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;


namespace mogate
{
	public interface IAction
	{
		bool Execute(GameTime gameTime);
	};

	public class Execute : IBehavior
	{
		public Type Behavior { get { return typeof(Execute); } }

		private Dictionary<string, Queue<IAction>> m_actions = new Dictionary<string, Queue<IAction>>();

		public void Start(IAction action, string tag = "")
		{
			if (!m_actions.ContainsKey (tag))
				m_actions [tag] = new Queue<IAction> ();

			m_actions[tag].Enqueue(action);
		}

		public void Update(GameTime gameTime)
		{
			var tags = new List<string>(m_actions.Keys);

			foreach (var tag in tags) {
				var q = m_actions [tag];
				if (q.Count == 0)
					continue;

				if (q.Peek ().Execute (gameTime))
					q.Dequeue ();
			}
		}

		public void Cancel(string tag = "")
		{
			m_actions.Remove (tag);
		}
	}
}

