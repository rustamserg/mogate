using System;
using Elizabeth;

namespace mogate
{
	public class Armor : IBehavior
	{
		public Type Behavior { get { return typeof(Armor); } }

		public int Defence { get; set; }
		public int ArchetypeID { get; set; }

		public Armor(int defence, int id = 0)
		{
			Defence = defence;
			ArchetypeID = id;
		}
	}
}

