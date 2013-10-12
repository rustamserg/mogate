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
		public string FrameName;

		public int FrameCount {
			get {
				return Sprite.Frames [FrameName].Count;
			}
		}

		public Rectangle DrawRect {
			get {
				return Sprite.GetFrame (FrameName, FrameId);
			}
		}

		public Drawable (SpriteFrame sprite, string frameName, Vector2 drawPos)
		{
			FrameId = 0;
			FrameName = frameName;
			Sprite = sprite;
			DrawPos = drawPos;
		}
	}
}

