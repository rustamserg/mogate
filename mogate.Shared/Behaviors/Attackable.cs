using Elizabeth;
using System;

namespace mogate
{
    public class Attackable : IBehavior
	{
		public Type Behavior { get { return typeof(Attackable); } }

		private Action<Entity, int, int> m_onAttack;

		public Attackable(Action<Entity, int, int> onAttack = null)
		{
			m_onAttack = onAttack;
		}

		public void Attack(Entity attacker, Entity defender)
		{
			if (attacker.Has<Attack> ()) {
				if (defender.Has<Health> ()) {
					int basedmg = attacker.Get<Attack> ().Damage;
					int criticaldmg = 0;
					if (attacker.Has<CriticalHit> ()) {
						if (Utils.DropChance (attacker.Get<CriticalHit> ().HitChance)) {
							criticaldmg = attacker.Get<CriticalHit> ().CriticalDamage;
						}
					}
					float attackMult = attacker.Has<AttackMultiplier> () ? attacker.Get<AttackMultiplier> ().Multiplier : 1;
					int defence = defender.Has<Armor> () ? defender.Get<Armor> ().Defence : 0;
					basedmg = (int)(basedmg * attackMult);
					criticaldmg = (int)(criticaldmg * attackMult);
					int damage = Math.Max(0, basedmg + criticaldmg - defence);

					if (m_onAttack != null)
						m_onAttack (attacker, damage, criticaldmg);

					defender.Get<Health> ().HP = Math.Max (0, defender.Get<Health> ().HP - damage);
				}
			}
		}
	}
}

