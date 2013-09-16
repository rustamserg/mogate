using System;

namespace mogate
{
	public class Health : IAspect
	{
		public Type Behavior { get { return typeof(Health); } }

		public int HP { get; set; }

		public Health(int hp)
		{
			HP = hp;
		}
	}
}

