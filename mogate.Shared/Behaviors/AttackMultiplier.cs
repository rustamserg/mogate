using System;

namespace mogate
{
	public class AttackMultiplier : IBehavior
	{
		public Type Behavior { get { return typeof(AttackMultiplier); } }

		public int Multiplier { get; set; }

		public AttackMultiplier (int mult)
		{
			Multiplier = mult;
		}
	}
}

