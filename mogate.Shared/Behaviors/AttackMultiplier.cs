using Elizabeth;
using System;

namespace mogate
{
    public class AttackMultiplier : IBehavior
	{
		public Type Behavior { get { return typeof(AttackMultiplier); } }

		public float Multiplier { get; set; }

		public AttackMultiplier (float mult)
		{
			Multiplier = mult;
		}
	}
}

