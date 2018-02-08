using Microsoft.Xna.Framework;
using System;

namespace Elizabeth
{
    public class Drawable : IBehavior
	{
		public Type Behavior { get { return typeof(Drawable); } }

		public Vector2 DrawPos;
		public Color DrawColor;
		public float DrawAlpha;
		public int ZOrder;

		public Drawable (Vector2 drawPos, int zorder = int.MaxValue)
		{
			DrawPos = drawPos;
			DrawColor = Color.White;
			DrawAlpha = 1.0f;
			ZOrder = zorder;
		}

		public Drawable (Vector2 drawPos, Color drawColor, int zorder = int.MaxValue)
		{
			DrawPos = drawPos;
			DrawColor = drawColor;
			DrawAlpha = 1.0f;
			ZOrder = zorder;
		}
	}
}

