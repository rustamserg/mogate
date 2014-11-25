using System;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class Sprite2D
	{
		public Texture2D Texture { get; private set; }
		public Rectangle Rect { get; private set; }
		public int Frames { get; private set; }
		public string Name { get; private set; }

		public Sprite2D(string n, Texture2D t, Rectangle r, int f)
		{
			Texture = t;
			Rect = r;
			Frames = f;
			Name = n;
		}

		public Sprite2D(string n, Texture2D t, Rectangle r) : this(n, t, r, r.Width / Globals.CELL_WIDTH) {}

		public Rectangle GetFrameRect(int frameId)
		{
			frameId = Math.Min (Frames - 1, frameId);
			return new Rectangle (Rect.X + frameId * Globals.CELL_WIDTH, Rect.Y, Globals.CELL_WIDTH, Globals.CELL_HEIGHT);
		}
	};

	public interface ISpriteSheets
	{
		Sprite2D GetSprite (string name);
		SpriteFont GetFont (string name);
	};

	public class SpriteSheets : DrawableGameComponent, ISpriteSheets
	{
		Dictionary<string, Sprite2D> m_sprites = new Dictionary<string, Sprite2D>();
		Dictionary<string, SpriteFont> m_fonts = new Dictionary<string, SpriteFont> ();

		public SpriteSheets (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			var texture = Game.Content.Load<Texture2D> ("Sprites/sprites");
			var stream = TitleContainer.OpenStream(@"Content/Sprites/sprites.plist");
			PList pinfo = new PList (stream);

			PList frames = pinfo ["frames"] as PList;
			foreach (var frmName in frames.Keys) {
				PList frame = frames [frmName] as PList;
				string texRect = frame ["textureRect"] as string;
				var sp = new Sprite2D (frmName, texture, Utils.RectangleFromString (texRect));
				m_sprites.Add (frmName, sp);
			}

			m_fonts ["SpriteFont1"] = Game.Content.Load<SpriteFont> ("Fonts/SpriteFont1");
		}

		public Sprite2D GetSprite(string name)
		{
			return m_sprites [name];
		}

		public SpriteFont GetFont(string name)
		{
			return m_fonts [name];
		}
	}
}