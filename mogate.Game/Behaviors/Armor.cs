using System;

namespace mogate
{
	public class Armor : IBehavior
	{
		public Type Behavior { get { return typeof(Armor); } }

		public int Value { get; set; }
		public int MaxArmor { get; private set; }

		public Armor(int armor)
		{
			Value = armor;
			MaxArmor= armor;
		}
	}
}

