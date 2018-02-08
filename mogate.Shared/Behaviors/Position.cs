using Elizabeth;
using Microsoft.Xna.Framework;
using System;


namespace mogate
{
    public class Position : IBehavior
	{
		public Type Behavior { get { return typeof(Position); } }

		public Point MapPos;

		public Position (int x, int y)
		{
			MapPos.X = x;
			MapPos.Y = y;
		}
	}
}

