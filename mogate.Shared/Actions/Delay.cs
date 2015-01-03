using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class Delay : IAction
	{
		float m_duration;
		int m_spent;

		public Delay (float duration)
		{
			m_duration = duration;
		}

		public bool Execute(GameTime gameTime)
		{
			m_spent += gameTime.ElapsedGameTime.Milliseconds;

			if (m_spent < m_duration) {
				return false;
			} else {
				m_spent = (int)(m_spent - m_duration);
				return true;
			}
		}

	}
}

