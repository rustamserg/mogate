using System;
using Elizabeth;

namespace mogate
{
	public class MoneyMultiplier : IBehavior
	{
		public Type Behavior { get { return typeof(MoneyMultiplier); } }

		public float Multiplier { get; set; }

		public MoneyMultiplier (float mult)
		{
			Multiplier = mult;
		}
	}
}

