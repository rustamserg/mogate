using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class Sprite : IBehavior
	{
		public Type Behavior { get { return typeof(Sprite); } }

		public Sprite2D Image;
		public int FrameId;

		private bool m_spriteSheet;

		public Rectangle DrawRect {
			get {
				if (m_spriteSheet) {
					return Image.GetFrameRect (FrameId);
				} else {
					return Image.Rect;
				}
			}
		}

		public Sprite (Sprite2D sprite, bool spriteSheet = true)
		{
			m_spriteSheet = spriteSheet;
			FrameId = 0;
			Image = sprite;
		}

	}
}

