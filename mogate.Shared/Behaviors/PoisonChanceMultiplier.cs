using Elizabeth;
using System;

namespace mogate
{
    public class PoisonChanceMultiplier : IBehavior
	{
		public Type Behavior { get { return typeof(PoisonChanceMultiplier); } }

		public float Multiplier { get; set; }

		public PoisonChanceMultiplier (float mult)
		{
			Multiplier = mult;
		}
	}
}

