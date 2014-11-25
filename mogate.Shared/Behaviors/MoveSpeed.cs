using System;

namespace mogate
{
	public class MoveSpeed : IBehavior
	{
		public Type Behavior { get { return typeof(MoveSpeed); } }

		public float Speed { get; set; }

		public MoveSpeed (float speed)
		{
			Speed = speed;
		}
	}
}

