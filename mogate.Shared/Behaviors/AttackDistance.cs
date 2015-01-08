using System;

namespace mogate
{
	public class AttackDistance : IBehavior
	{
		public Type Behavior { get { return typeof(AttackDistance); } }

		public int Distance { get; set; }

		public AttackDistance (int distance)
		{
			Distance = distance;
		}
	}
}

