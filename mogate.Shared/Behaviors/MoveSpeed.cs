using Elizabeth;
using System;

namespace mogate
{
    public class MoveSpeed : IBehavior
	{
		public Type Behavior { get { return typeof(MoveSpeed); } }

		public int Speed { get; set; }

		public MoveSpeed (int speed)
		{
			Speed = speed;
		}
	}
}

