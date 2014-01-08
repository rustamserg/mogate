using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class Drawable : IBehavior
	{
		public Type Behavior { get { return typeof(Drawable); } }

		public Sprite2D Sprite;
		public Vector2 DrawPos;
		public Color DrawColor;
		public int FrameId;

		public Rectangle DrawRect {
			get {
				return Sprite.GetFrameRect (FrameId);
			}
		}

		public Drawable (Sprite2D sprite, Vector2 drawPos)
		{
			FrameId = 0;
			Sprite = sprite;
			DrawPos = drawPos;
			DrawColor = Color.White;
		}
	}
}

