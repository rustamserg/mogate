using System;

namespace mogate
{
	public class Attack : IBehavior
	{
		public Type Behavior { get { return typeof(Attack); } }

		public int Damage { get; set; }
		public int ArchetypeID { get; set; }

		public Attack (int damage, int id = 0)
		{
			Damage = damage;
			ArchetypeID = id;
		}
	}
}

