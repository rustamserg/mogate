using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class MoveSpriteTo : IAction
	{
		Entity m_moved;
		int m_speed;
		Point m_dest;

		public MoveSpriteTo (Entity moved, Point to, int speed)
		{
			m_moved = moved;
			m_speed = speed;
			m_dest = to;
		}

		public bool Execute()
		{
			if (m_moved.Get<Drawable>().DrawPos.X == m_dest.X && m_moved.Get<Drawable>().DrawPos.Y == m_dest.Y)
				return true;

			if (m_moved.Get<Drawable>().DrawPos.X < m_dest.X)
				m_moved.Get<Drawable>().DrawPos.X += m_speed;
			if (m_moved.Get<Drawable>().DrawPos.X > m_dest.X)
				m_moved.Get<Drawable>().DrawPos.X -= m_speed;
			if (m_moved.Get<Drawable>().DrawPos.Y < m_dest.Y)
				m_moved.Get<Drawable>().DrawPos.Y += m_speed;
			if (m_moved.Get<Drawable>().DrawPos.Y > m_dest.Y)
				m_moved.Get<Drawable>().DrawPos.Y -= m_speed;

			return false;
		}
	}
}

