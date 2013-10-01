using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class Drawable : IBehavior
	{
		public Type Behavior { get { return typeof(Drawable); } }

		public Texture2D SpriteSheet;
		public Rectangle SpriteRect;
		public Vector2 DrawPos;

		public Drawable (Texture2D spriteSheet, Rectangle spriteRect, Point drawPos)
		{
			SpriteSheet = spriteSheet;
			SpriteRect = spriteRect;
			DrawPos.X = drawPos.X;
			DrawPos.Y = drawPos.Y;
		}
	}
}

