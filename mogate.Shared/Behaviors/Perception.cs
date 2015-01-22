using System;
using Elizabeth;

namespace mogate
{
	public class Perception : IBehavior
	{
		public Type Behavior { get { return typeof(Perception); } }
		public int AlertDistance { get; private set; }

		public Perception (int alert)
		{
			AlertDistance = alert;
		}
	}
}

