using System;
using Elizabeth;

namespace mogate
{
	public class AttackSpeed : IBehavior
	{
		public Type Behavior { get { return typeof(AttackSpeed); } }

		public int Speed { get; set; }

		public AttackSpeed (int speed)
		{
			Speed = speed;
		}
	}
}

