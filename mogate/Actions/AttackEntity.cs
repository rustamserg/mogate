using System;

namespace mogate
{
	public class AttackEntity : IAction
	{
		private Entity m_attacked;
		private int m_damage;

		public AttackEntity (Entity attacked, int damage)
		{
			m_attacked = attacked;
			m_damage = damage;
		}

		public bool Execute()
		{
			m_attacked.Get<Health> ().HP = Math.Max (0, m_attacked.Get<Health> ().HP - m_damage);
			return true;
		}
	}
}

