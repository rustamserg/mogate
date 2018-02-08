using Elizabeth;
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
					float poisonChanceMult = defender.Has<PoisonChanceMultiplier> () ? defender.Get<PoisonChanceMultiplier> ().Multiplier : 1;
					if (Utils.DropChance ((int)(attacker.Get<Poison> ().Chance * poisonChanceMult))) {
						if (defender.Has<Execute> ()) {
							var poisonLoop = new Loop (new DoPoisonEntity (defender, attacker.Get<Poison> ().Damage, m_onPoisoned),
									                attacker.Get<Poison> ().Speed);
							defender.Get<Execute> ().AddNew (poisonLoop, "poison_effect");
							IsPoisoned = true;
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

