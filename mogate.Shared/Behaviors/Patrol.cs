using System;

namespace mogate
{
	public class Patrol : IBehavior
	{
		public Type Behavior { get { return typeof(Patrol); } }

		public int Steps { get; set; }

		public int MinSteps { get; private set; }
		public int MaxSteps { get; private set; }

		public Patrol (int minSteps, int maxSteps)
		{
			Steps = 0;
			MinSteps = minSteps;
			MaxSteps = maxSteps;
		}
	}
}

