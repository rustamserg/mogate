using System;

namespace mogate
{
	public class CriticalHit : IBehavior
	{
		public Type Behavior { get { return typeof(CriticalHit); } }

		public int HitChance { get; private set; }
		public int CriticalDamage { get; private set; }

		public CriticalHit (int hitChance, int criticalDamage)
		{
			HitChance = hitChance;
			CriticalDamage = criticalDamage;
		}
	}
}

