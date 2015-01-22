using System;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if __IOS__
using CocosSharp;
#endif


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

		private readonly int m_frameWidth;
		private readonly int m_frameHeight;

		public SpriteSheets (Game game, int frameWidth, int frameHeight) : base(game)
		{
			m_frameWidth = frameWidth;
			m_frameHeight = frameHeight;
		}

		protected override void LoadContent ()
		{
			Texture2D texture = Game.Content.Load<Texture2D> ("Sprites/sprites");
			var stream = TitleContainer.OpenStream(@"Content/Sprites/sprites.plist");

			#if __IOS__
			PlistDocument pinfo = new PlistDocument(stream);
			PlistDictionary frames = pinfo.Root.AsDictionary["frames"].AsDictionary;

			foreach (var frame in frames) {
				string spriteName = frame.Key.Split('.')[0];
				string texRect = frame.Value.AsDictionary["frame"].AsString;
				var sp = new Sprite2D (spriteName, texture, Utils.RectangleFromString (texRect));
				m_sprites.Add (spriteName, sp);
			}
			#else
			PList pinfo = new PList (stream);
			PList frames = pinfo ["frames"] as PList;

			foreach (var frmName in frames.Keys) {
				PList frame = frames [frmName] as PList;
				string texRect = frame ["frame"] as string;
				string spriteName = frmName.Split('.')[0];
				var sp = new Sprite2D (spriteName, texture, RectangleFromString (texRect), m_frameWidth, m_frameHeight);
				m_sprites.Add (spriteName, sp);
			}
			#endif

			#if __IOS__
			m_fonts ["SpriteFont1"] = Game.Content.Load<SpriteFont> ("Fonts/arial-22");
			#else
			m_fonts ["SpriteFont1"] = Game.Content.Load<SpriteFont> ("Fonts/SpriteFont1");
			#endif
		}

		public Sprite2D GetSprite(string name)
		{
			return m_sprites [name];
		}

		public SpriteFont GetFont(string name)
		{
			return m_fonts [name];
		}

		private Rectangle RectangleFromString(string rect)
		{
			// convert {{x, y}, {w, h}} to rect
			string parsed = rect.Replace ("{", "").Replace ("}", "");
			string[] dt = parsed.Split (',');
			return new Rectangle (int.Parse (dt [0]), int.Parse (dt [1]), int.Parse (dt [2]), int.Parse (dt [3]));
		}
	}
}