using System;

namespace mogate
{
	public class Poisonable : IBehavior
	{
		public Type Behavior { get { return typeof(Poisonable); } }

		private Action<Entity> m_onPoisoned;

		public Poisonable(Action<Entity> onPoisoned = null)
		{
			m_onPoisoned = onPoisoned;
		}

		public void TryPoison(Entity attacker, Entity defender)
		{
			if (attacker.Has<Poison> ()) {
				if (defender.Has<Health> ()) {
					if (Utils.DropChance (attacker.Get<Poison> ().Chance)) {
						if (m_onPoisoned != null)
							m_onPoisoned (attacker);

						if (defender.Has<Execute> ()) {
							var poisonLoop = new Loop (new DoPoisonEntity (defender, attacker.Get<Poison> ().Damage),
								                 attacker.Get<Poison> ().Speed);
							defender.Get<Execute> ().AddNew (poisonLoop, "poison_effect");
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

