using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public interface ISpriteSheets
	{
		Texture2D GetSprite (string name);
	};

	public class SpriteSheets : DrawableGameComponent, ISpriteSheets
	{
		Dictionary<string, Texture2D> m_textures = new Dictionary<string, Texture2D>();

		public SpriteSheets (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_textures ["hero"] = Game.Content.Load<Texture2D> ("hero");
		}

		public Texture2D GetSprite(string name)
		{
			return m_textures [name];
		}
	}
}

