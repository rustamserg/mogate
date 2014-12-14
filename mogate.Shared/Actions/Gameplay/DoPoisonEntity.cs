using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class DoPoisonEntity : IAction
	{
		private Entity m_poisoned;
		private int m_damage;

		public DoPoisonEntity (Entity poisoned, int damage)
		{
			m_poisoned = poisoned;
			m_damage = damage;
		}

		public bool Execute(GameTime gameTime)
		{
			if (m_poisoned.Has<Health> ())
				m_poisoned.Get<Health> ().HP = Math.Max (0, m_poisoned.Get<Health> ().HP - m_damage);

			return true;
		}
	}
}

