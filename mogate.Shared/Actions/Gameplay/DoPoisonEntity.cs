using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class DoPoisonEntity : IAction
	{
		private Entity m_poisoned;
		private int m_damage;
		private Action<Entity, int> m_onPoisoned;

		public DoPoisonEntity (Entity poisoned, int damage, Action<Entity, int> onPoisoned = null)
		{
			m_poisoned = poisoned;
			m_damage = damage;
			m_onPoisoned = onPoisoned;
		}

		public bool Execute(GameTime gameTime)
		{
			if (m_poisoned.Has<Health> ()) {
				if (m_onPoisoned != null) {
					m_onPoisoned (m_poisoned, m_damage);
				}
				m_poisoned.Get<Health> ().HP = Math.Max (0, m_poisoned.Get<Health> ().HP - m_damage);
			}

			return true;
		}
	}
}

