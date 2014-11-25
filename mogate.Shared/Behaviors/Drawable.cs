using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class Drawable : IBehavior
	{
		public Type Behavior { get { return typeof(Drawable); } }

		public Vector2 DrawPos;
		public Color DrawColor;
		public float DrawAlpha;

		public Drawable (Vector2 drawPos)
		{
			DrawPos = drawPos;
			DrawColor = Color.White;
			DrawAlpha = 1.0f;
		}

		public Drawable (Vector2 drawPos, Color drawColor)
		{
			DrawPos = drawPos;
			DrawColor = drawColor;
			DrawAlpha = 1.0f;
		}
	}
}

