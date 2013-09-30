using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class MoveTo : IAction
	{
		Entity m_moved;
		int m_speed;

		public MoveTo (Entity moved, Point to, int speed)
		{
			m_moved = moved;
			m_speed = speed;
			m_moved.Get<Position> ().MapPos = to;
		}

		public bool Execute()
		{
			if (m_moved.Get<Position>().MapPos.X * 32 == m_moved.Get<Position>().DrawPos.X
			    && m_moved.Get<Position>().MapPos.Y * 32 == m_moved.Get<Position>().DrawPos.Y) {
				return true;
			} else {
				if (m_moved.Get<Position>().DrawPos.X < m_moved.Get<Position>().MapPos.X * 32)
					m_moved.Get<Position>().DrawPos.X += m_speed;
				if (m_moved.Get<Position>().DrawPos.X > m_moved.Get<Position>().MapPos.X * 32)
					m_moved.Get<Position>().DrawPos.X -= m_speed;
				if (m_moved.Get<Position>().DrawPos.Y < m_moved.Get<Position>().MapPos.Y * 32)
					m_moved.Get<Position>().DrawPos.Y += m_speed;
				if (m_moved.Get<Position>().DrawPos.Y > m_moved.Get<Position>().MapPos.Y * 32)
					m_moved.Get<Position>().DrawPos.Y -= m_speed;
			}
			return false;
		}
	}
}

