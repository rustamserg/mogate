using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class Loop : IAction
	{
		IAction m_looped;

		public Loop (IAction action)
		{
			m_looped = action;
		}

		public bool Execute(GameTime gameTime)
		{
			m_looped.Execute (gameTime);
			return false;
		}
	}
}

