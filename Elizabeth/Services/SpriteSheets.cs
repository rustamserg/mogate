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
	public interface ISpriteSheets
	{
		void AddSpriteSheet (string plistName, string texture, int frameWidth, int frameHeight);
		void AddSpriteFont (string fontName, string texture);

		Sprite2D GetSprite (string name);
		SpriteFont GetFont (string name);
	};

	public class SpriteSheets : DrawableGameComponent, ISpriteSheets
	{
		struct SheetData {
			public string PList;
			public string Texture;
			public int FrameWidth;
			public int FrameHeight;
		};
		struct FontData {
			public string Name;
			public string Texture;
		};

		Dictionary<string, Sprite2D> m_sprites = new Dictionary<string, Sprite2D>();
		Dictionary<string, SpriteFont> m_fonts = new Dictionary<string, SpriteFont> ();

		private List<SheetData> m_addedSheets = new List<SheetData>();
		private List<FontData> m_addedFonts = new List<FontData> ();

		public SpriteSheets(Game game) : base(game)
		{
		}

		public void AddSpriteSheet (string plistName, string texture, int frameWidth, int frameHeight)
		{
			var sd = new SheetData {
				PList = plistName,
				Texture = texture,
				FrameWidth = frameWidth,
				FrameHeight = frameHeight
			};
			m_addedSheets.Add (sd);
		}

		public void AddSpriteFont (string fontName, string texture)
		{
			var fd = new FontData {
				Name = fontName,
				Texture = texture
			};
			m_addedFonts.Add (fd);
		}

		protected override void LoadContent ()
		{
			foreach (var sd in m_addedSheets) {
				LoadSheetData (sd);
			}
			foreach (var fd in m_addedFonts) {
				LoadFontData (fd);
			}
			m_addedFonts.Clear ();
			m_addedSheets.Clear ();
		}

		public Sprite2D GetSprite(string name)
		{
			return m_sprites [name];
		}

		public SpriteFont GetFont(string name)
		{
			return m_fonts [name];
		}

		private void LoadSheetData(SheetData sheetData)
		{
			var texture = Game.Content.Load<Texture2D> (sheetData.Texture);
			var stream = TitleContainer.OpenStream(sheetData.PList);

			#if __IOS__
			PlistDocument pinfo = new PlistDocument(stream);
			PlistDictionary frames = pinfo.Root.AsDictionary["frames"].AsDictionary;

			foreach (var frame in frames) {
				string spriteName = frame.Key.Split('.')[0];
				string texRect = frame.Value.AsDictionary["frame"].AsString;
				var sp = new Sprite2D (spriteName, texture, RectangleFromString (texRect), sheetData.FrameWidth, sheetData.FrameHeight);
				m_sprites.Add (spriteName, sp);
			}
			#else
			PList pinfo = new PList (stream);
			PList frames = pinfo ["frames"] as PList;

			foreach (var frmName in frames.Keys) {
				PList frame = frames [frmName] as PList;
				string texRect = frame ["frame"] as string;
				string spriteName = frmName.Split('.')[0];
				var sp = new Sprite2D (spriteName, texture, RectangleFromString (texRect), sheetData.FrameWidth, sheetData.FrameHeight);
				m_sprites.Add (spriteName, sp);
			}
			#endif
		}

		private void LoadFontData(FontData fontData)
		{
			m_fonts [fontData.Name] = Game.Content.Load<SpriteFont> (fontData.Texture);
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