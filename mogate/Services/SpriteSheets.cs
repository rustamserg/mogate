using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class SpriteFrame
	{
		public struct Frame
		{
			public Rectangle Rect;
			public int Count;

			public Frame(Rectangle r, int c)
			{
				Rect = r;
				Count = c;
			}
		};

		public Texture2D Texture;
		public Dictionary<string, Frame> Frames = new Dictionary<string, Frame>();

		public Rectangle GetFrame(string frameName, int frameId)
		{
			var frame = Frames [frameName];
			frameId = Math.Min (frame.Count - 1, frameId);
			return new Rectangle (frame.Rect.X + frameId * 32, frame.Rect.Y, 32, 32);
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
			sp.Texture = Game.Content.Load<Texture2D> ("hero");
			sp.Frames.Add ("move", new SpriteFrame.Frame (new Rectangle (0, 0, 32 * 8, 32), 8));
			sp.Frames.Add ("idle", new SpriteFrame.Frame (new Rectangle (0, 32, 32 * 8, 32), 8));
			m_textures ["hero"] = sp;

			sp = new SpriteFrame ();
			sp.Texture = Game.Content.Load<Texture2D> ("monster");
			sp.Frames.Add ("idle", new SpriteFrame.Frame (new Rectangle (0, 0, 32, 32), 1));
			m_textures ["monster"] = sp;

			sp = new SpriteFrame ();
			sp.Texture = Game.Content.Load<Texture2D> ("sword");
			sp.Frames.Add ("damage", new SpriteFrame.Frame (new Rectangle (0, 0, 32, 32), 1));
			m_textures ["effects"] = sp;
		}

		public SpriteFrame GetSprite(string name)
		{
			return m_textures [name];
		}
	}
}