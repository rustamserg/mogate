using System;

namespace mogate
{
	public class Attackable : IBehavior
	{
		public Type Behavior { get { return typeof(Attackable); } }

		private Action<Entity, int> m_onAttack;

		public Attackable(Action<Entity, int> onAttack = null)
		{
			m_onAttack = onAttack;
		}

		public void Attack(Entity attacker, Entity defender)
		{
			if (attacker.Has<Attack> ()) {
				if (defender.Has<Health> ()) {
					int basedmg = attacker.Get<Attack> ().Damage;
					int damage = basedmg + Utils.ThrowDice (basedmg);

					if (m_onAttack != null)
						m_onAttack (attacker, damage);

					defender.Get<Health> ().HP = Math.Max (0, defender.Get<Health> ().HP - damage);
				}
			}
		}
	}
}

