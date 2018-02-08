using Elizabeth;
using System;

namespace mogate
{
    public class Poison : IBehavior
	{
		public Type Behavior { get { return typeof(Poison); } }

		public int Damage { get; set; }
		public int Chance { get; set; }
		public int Speed { get; set; }

		public Poison (int damage, int chance, int speed)
		{
			Damage = damage;
			Chance = chance;
			Speed = speed;
		}
	}
}