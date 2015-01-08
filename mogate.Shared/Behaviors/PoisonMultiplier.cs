using System;

namespace mogate
{
	public class PoisonMultiplier : IBehavior
	{
		public Type Behavior { get { return typeof(PoisonMultiplier); } }

		public int Multiplier { get; set; }

		public PoisonMultiplier (int mult)
		{
			Multiplier = mult;
		}
	}
}

