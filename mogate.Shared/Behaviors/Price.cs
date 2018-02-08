using Elizabeth;
using System;

namespace mogate
{
    public class Price : IBehavior
	{
		public Type Behavior { get { return typeof(Price); } }

		public int Cost { get; private set; }

		public Price (int cost)
		{
			Cost = cost;
		}
	}
}

