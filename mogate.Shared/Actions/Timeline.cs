using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class Timeline : IAction
	{
		IAction m_timeline;
		float m_duration;
		int m_spent;

		public Timeline (IAction timeline, float duration)
		{
			m_timeline = timeline;
			m_duration = duration;
		}

		public bool Execute(GameTime gameTime)
		{
			m_spent += gameTime.ElapsedGameTime.Milliseconds;

			if (m_spent < m_duration) {
				m_timeline.Execute (gameTime);
				return false;
			} else {
				m_spent = (int)(m_spent - m_duration);
				return true;
			}
		}
	}
}

