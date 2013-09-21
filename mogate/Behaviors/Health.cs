using System;

namespace mogate
{
	public class Health : IBehavior
	{
		public Type Behavior { get { return typeof(Health); } }

		public int HP { get; set; }
		public int MaxHP { get; private set; }

		public Health(int hp)
		{
			HP = hp;
			MaxHP = hp;
		}
	}
}

