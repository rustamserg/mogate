using System;

namespace mogate
{
	public class AttackDistanceMultiplier : IBehavior
	{
		public Type Behavior { get { return typeof(AttackDistanceMultiplier); } }

		public float Multiplier { get; set; }

		public AttackDistanceMultiplier (float mult)
		{
			Multiplier = mult;
		}
	}
}

