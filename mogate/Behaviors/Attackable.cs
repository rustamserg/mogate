using System;

namespace mogate
{
	public class Attackable : IBehavior
	{
		public Type Behavior { get { return typeof(Attackable); } }

		private Action<Entity> m_onAttack;

		public Attackable(Action<Entity> onAttack = null)
		{
			m_onAttack = onAttack;
		}

		public void Attack(Entity attacker, Entity defender)
		{
			if (attacker.Has<Attack> ()) {
				if (defender.Has<Health> ()) {
					defender.Get<Health> ().HP = Math.Max (0, defender.Get<Health> ().HP - attacker.Get<Attack> ().Damage);
				}
				if (m_onAttack != null)
					m_onAttack (attacker);
			}
		}
	}
}

