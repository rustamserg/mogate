using System;

namespace mogate
{
	public class Patrol : IBehavior
	{
		public Type Behavior { get { return typeof(Patrol); } }

		public Utils.Direction Direction { get; set; }
		public int Steps { get; set; }

		public Patrol ()
		{
			Direction = Utils.Direction.Down;
			Steps = 0;
		}
	}
}

