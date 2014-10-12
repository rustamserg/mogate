using System;

namespace mogate
{
	public class PointLight : IBehavior
	{
		public Type Behavior { get { return typeof(PointLight); } }

		public int Distance { get; set; }

		public PointLight(int dist)
		{
			Distance = dist;
		}
	}
}

