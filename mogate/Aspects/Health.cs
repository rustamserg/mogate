using System;

namespace mogate
{
	public class Health : IAspect
	{
		public int HP { get; set; }
		public string Name { get { return "health"; } }

		public Health(int hp)
		{
			HP = hp;
		}
	}
}

