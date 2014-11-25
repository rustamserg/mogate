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

				if (m_onAttack != null)
					m_onAttack (attacker);

				int armor = 0;
				if (defender.Has<Armor> () && defender.Get<Armor> ().Value > 0) {
					armor = defender.Get<Armor> ().Value;
				}
				
				if (defender.Has<Health> ()) {
					int damage = Utils.ThrowDice (attacker.Get<Attack> ().Damage);
					damage = Math.Max (0, damage - armor);
					defender.Get<Health> ().HP = Math.Max (0, defender.Get<Health> ().HP - damage);
				}
			}
		}
	}
}

