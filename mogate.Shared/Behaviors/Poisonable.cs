using System;

namespace mogate
{
	public class Poisonable : IBehavior
	{
		public Type Behavior { get { return typeof(Poisonable); } }

		private Action<Entity, int> m_onPoisoned;
		public bool IsPoisoned { get; private set; }

		public Poisonable(Action<Entity, int> onPoisoned = null)
		{
			m_onPoisoned = onPoisoned;
			IsPoisoned = false;
		}

		public void TryPoison(Entity attacker, Entity defender)
		{
			if (attacker.Has<Poison> ()) {
				if (defender.Has<Health> ()) {
					if (Utils.DropChance (attacker.Get<Poison> ().Chance)) {
						if (defender.Has<Execute> ()) {
							int poisonMult = defender.Has<PoisonMultiplier> () ? defender.Get<PoisonMultiplier> ().Multiplier : 1;
							int poisonDamage = attacker.Get<Poison> ().Damage * poisonMult;

							if (poisonDamage > 0) {
								var poisonLoop = new Loop (new DoPoisonEntity (defender, attacker.Get<Poison> ().Damage, m_onPoisoned),
									                attacker.Get<Poison> ().Speed);
								defender.Get<Execute> ().AddNew (poisonLoop, "poison_effect");
								IsPoisoned = true;
							}
						}
					}
				}
			}
		}

		public void CancelPoison(Entity defender)
		{
			if (defender.Has<Execute> ()) {
				defender.Get<Execute> ().Cancel ("poison_effect");
			}
		}
	}
}

