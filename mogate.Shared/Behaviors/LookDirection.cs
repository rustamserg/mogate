using System;
using Elizabeth;

namespace mogate
{
	public class LookDirection : IBehavior
	{
		public Type Behavior { get { return typeof(LookDirection); } }

		public Utils.Direction Direction { get; set; }

		public LookDirection (Utils.Direction direction)
		{
			Direction = direction;
		}
	}
}

