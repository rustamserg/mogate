using System;
using Elizabeth;

namespace mogate
{
	public class Attack : IBehavior
	{
		public Type Behavior { get { return typeof(Attack); } }

		public int Damage { get; set; }
		public int Distance  { get; set; }
		public int ArchetypeID { get; set; }

		public Attack (int damage, int distance, int id = 0)
		{
			Damage = damage;
			Distance = distance;
			ArchetypeID = id;
		}
	}
}

