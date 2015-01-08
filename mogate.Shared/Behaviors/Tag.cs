using System;

namespace mogate
{
	public class Tag : IBehavior
	{
		public Type Behavior { get { return typeof(Tag); } }

		public int ID { get; set; }

		public Tag (int id)
		{
			ID = id;
		}
	}
}

