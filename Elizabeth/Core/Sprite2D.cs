using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Elizabeth
{
    public class Sprite2D
	{
		public Texture2D Texture { get; private set; }
		public Rectangle Rect { get; private set; }
		public int Frames { get; private set; }
		public string Name { get; private set; }

		private readonly int m_frameWidth;
		private readonly int m_frameHeight;

		public Sprite2D(string n, Texture2D t, Rectangle r, int f, int fw, int fh)
		{
			Texture = t;
			Rect = r;
			Frames = f;
			Name = n;
			m_frameWidth = fw;
			m_frameHeight = fh;
		}

		public Sprite2D(string n, Texture2D t, Rectangle r, int fw, int fh) : this(n, t, r, r.Width / fw, fw, fh) {}

		public Rectangle GetFrameRect(int frameId)
		{
			frameId = Math.Min (Frames - 1, frameId);
			return new Rectangle (Rect.X + frameId * m_frameWidth, Rect.Y, m_frameWidth, m_frameHeight);
		}
	}
}