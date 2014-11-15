using System;

namespace mogate
{
	public class DirectLight : IBehavior
	{
		public Type Behavior { get { return typeof(DirectLight); } }

		public int Distance { get; set; }
		public Utils.Direction Direction { get; set; }

		public DirectLight (int dist, Utils.Direction dir)
		{
			Distance = dist;
			Direction = dir;
		}
	}
}

