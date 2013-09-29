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

		public Drawable (Texture2D spriteSheet, Rectangle spriteRect)
		{
			SpriteSheet = spriteSheet;
			SpriteRect = spriteRect;
		}
	}
}

