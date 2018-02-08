using Elizabeth;
using System;

namespace mogate
{
    public class Loot : IBehavior
	{
		public Type Behavior { get { return typeof(Loot); } }

		public int Drop { get; set; }

		public Loot (int drop)
		{
			Drop = drop;
		}
	}
}

