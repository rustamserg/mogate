using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class Sprite : IBehavior
	{
		public Type Behavior { get { return typeof(Sprite); } }

		public Sprite2D Image;
		public int FrameId;

		public Rectangle DrawRect {
			get {
				return Image.GetFrameRect (FrameId);
			}
		}

		public Sprite (Sprite2D sprite)
		{
			FrameId = 0;
			Image = sprite;
		}

	}
}

