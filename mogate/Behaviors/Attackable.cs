using System;

namespace mogate
{
	public class Attackable : IBehavior
	{
		public Type Behavior { get { return typeof(Attackable); } }

		public void Attack(Entity attacker, Entity defender)
		{
			if (attacker.Get<Attack> () != null) {
				if (defender.Get<Health> () != null) {
					defender.Get<Health> ().HP = Math.Max (0, defender.Get<Health> ().HP - attacker.Get<Attack> ().Damage);
				}
			}
		}
	}
}

