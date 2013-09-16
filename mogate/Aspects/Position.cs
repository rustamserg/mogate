using System;
using Microsoft.Xna.Framework;


namespace mogate
{
	public class Position : IAspect
	{
		public Type Behavior { get { return typeof(Position); } }

		public Point MapPos;
		public Vector2 DrawPos;

		public Position (int x, int y)
		{
			MapPos.X = x;
			MapPos.Y = y;
			DrawPos.X = MapPos.X * 32;
			DrawPos.Y = MapPos.Y * 32;
		}
	}
}

