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

		public void AddNew(IAction action, string tag = "")
		{
			Cancel (tag);
			Add (action, tag);
		}

		public void Add(IAction action, string tag = "")
		{
			if (!m_actions.ContainsKey (tag))
				m_actions [tag] = new Queue<IAction> ();

			m_actions[tag].Enqueue(action);
		}

		public void Update(GameTime gameTime)
		{
			var tags = new List<string>(m_actions.Keys);

			foreach (var tag in tags) {
				Queue<IAction> q;
				if (!m_actions.TryGetValue (tag, out q))
					continue;

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

		public void CancelAll()
		{
			m_actions.Clear ();
		}
	}
}

