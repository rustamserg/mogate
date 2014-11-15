using System;

namespace mogate
{
	public class AttackSpeed : IBehavior
	{
		public Type Behavior { get { return typeof(AttackSpeed); } }

		public float Speed { get; set; }

		public AttackSpeed (float speed)
		{
			Speed = speed;
		}
	}
}

