using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class Spawn : IAction
	{
		private List<IAction> m_spawn = new List<IAction>();

		public void Add(IAction action)
		{
			m_spawn.Add (action);
		}

		public bool Execute (GameTime gameTime)
		{
			if (m_spawn.Count == 0)
				return true;

			List<IAction> spawn = new List<IAction> ();
			foreach (var action in m_spawn) {
				if (!action.Execute (gameTime))
					spawn.Add (action);
			}
			m_spawn = spawn;
			return (m_spawn.Count == 0);
		}
	}
}

