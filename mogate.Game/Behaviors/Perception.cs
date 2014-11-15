using System;

namespace mogate
{
	public enum PerceptionType { Directional, Circular };

	public class Perception : IBehavior
	{
		public Type Behavior { get { return typeof(Perception); } }

		public PerceptionType Type { get; private set; }
		public int AlertDistance { get; private set; }

		public Perception (PerceptionType type, int alert)
		{
			Type = type;
			AlertDistance = alert;
		}
	}
}

