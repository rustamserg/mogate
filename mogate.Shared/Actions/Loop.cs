using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class Loop : IAction
	{
		IAction m_looped;
		int m_delay;
		int m_lastTime;

		public Loop (IAction action, int delay = 0)
		{
			m_looped = action;
			m_delay = delay;
			m_lastTime = 0;
		}

		public bool Execute(GameTime gameTime)
		{
			bool canExecute = true;

			if (m_delay > 0) {
				if (m_lastTime == 0) {
					m_lastTime = gameTime.ElapsedGameTime.Milliseconds;
				}
				var currTime = gameTime.ElapsedGameTime.Milliseconds;
				if (currTime - m_lastTime >= m_delay) {
					m_lastTime = currTime;
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

