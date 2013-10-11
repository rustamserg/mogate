using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class Drawable : IBehavior
	{
		public Type Behavior { get { return typeof(Drawable); } }

		public SpriteFrame Sprite;
		public Vector2 DrawPos;
		public int FrameId;

		public Drawable (SpriteFrame sprite, Vector2 drawPos)
		{
			FrameId = 0;
			Sprite = sprite;
			DrawPos = drawPos;
		}
	}
}

