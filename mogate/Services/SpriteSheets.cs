using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public struct SpriteFrame
	{
		public Texture2D Texture;
		public Rectangle Rect;
		public int FrameCount;

		public Rectangle GetFrame(int frameId)
		{
			frameId = Math.Min (FrameCount - 1, frameId);
			var fr = new Rectangle (0, 0, 32, 32);
			fr.X = Rect.X + frameId * 32;
			fr.Y = Rect.Y;
			return fr;
		}
	};

	public interface ISpriteSheets
	{
		SpriteFrame GetSprite (string name);
	};

	public class SpriteSheets : DrawableGameComponent, ISpriteSheets
	{
		Dictionary<string, SpriteFrame> m_textures = new Dictionary<string, SpriteFrame>();

		public SpriteSheets (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			var sp = new SpriteFrame ();
			sp.Rect = new Rectangle (0, 0, 32 * 8, 32 * 2);
			sp.FrameCount = 8;
			sp.Texture = Game.Content.Load<Texture2D> ("hero");

			m_textures ["hero"] = sp;

			sp = new SpriteFrame ();
			sp.Rect = new Rectangle (0, 0, 32, 32);
			sp.FrameCount = 1;
			sp.Texture = Game.Content.Load<Texture2D> ("monster");

			m_textures ["monster"] = sp;
		}

		public SpriteFrame GetSprite(string name)
		{
			return m_textures [name];
		}
	}
}