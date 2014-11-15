using System;

namespace mogate
{
	public class AllowedMapArea : IBehavior
	{
		public Type Behavior { get { return typeof(AllowedMapArea); } }

		public MapGridTypes.ID Area { get; private set; }

		public AllowedMapArea (MapGridTypes.ID area)
		{
			Area = area;
		}
	}
}

