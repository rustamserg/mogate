using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class Loop : IAction
	{
		IAction m_looped;
		int m_delay;
		int m_spent;

		public Loop (IAction action, int delay = 0)
		{
			m_looped = action;
			m_delay = delay;
			m_spent = 0;
		}

		public bool Execute(GameTime gameTime)
		{
			bool canExecute = true;

			if (m_delay > 0) {
				m_spent += gameTime.ElapsedGameTime.Milliseconds;
				if (m_spent >= m_delay) {
					m_spent = m_spent - m_delay;
				} else {
					canExecute = false;
				}
			}

			if (canExecute) {
				m_looped.Execute (gameTime);
			}
			return false;
		}
	}
}

