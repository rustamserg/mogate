using System;

namespace mogate
{
	public class MoneyMultiplier : IBehavior
	{
		public Type Behavior { get { return typeof(MoneyMultiplier); } }

		public int Multiplier { get; set; }

		public MoneyMultiplier (int mult)
		{
			Multiplier = mult;
		}
	}
}

