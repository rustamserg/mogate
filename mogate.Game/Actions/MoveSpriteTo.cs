using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class MoveSpriteTo : IAction
	{
		Entity m_moved;
		Vector2 m_to;
		Vector2 m_from;
		float m_duration;
		int m_spent;
		bool m_first;

		public MoveSpriteTo (Entity moved, Vector2 to, float duration)
		{
			m_moved = moved;
			m_to = to;
			m_duration = duration;
			m_first = true;
		}

		public bool Execute(GameTime gameTime)
		{
			if (m_first) {
				m_first = false;
				m_from = m_moved.Get<Drawable> ().DrawPos;
			}

			m_spent += gameTime.ElapsedGameTime.Milliseconds;

			if (m_spent >= m_duration) {
				m_moved.Get<Drawable> ().DrawPos = m_to;
			} else {
				float t = m_spent / m_duration;
				m_moved.Get<Drawable> ().DrawPos = Vector2.Lerp(m_from, m_to, t);
			}

			return (m_moved.Get<Drawable> ().DrawPos == m_to);
		}
	}
}

