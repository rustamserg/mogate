using Elizabeth;
using System;

namespace mogate
{
    public class CriticalHit : IBehavior
	{
		public Type Behavior { get { return typeof(CriticalHit); } }

		public int HitChance { get; set; }
		public int CriticalDamage { get; set; }

		public CriticalHit (int hitChance = 0, int criticalDamage = 0)
		{
			HitChance = hitChance;
			CriticalDamage = criticalDamage;
		}
	}
}

