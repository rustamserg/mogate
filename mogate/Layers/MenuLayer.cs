using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class MenuLayer : Layer
	{
		private SpriteFont m_font;

		public MenuLayer (Game game, string name, int z) : base(game, name, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_font = sprites.GetFont ("SpriteFont1");
		}

		protected override void OnPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.DrawString (m_font, "Press space ...", new Vector2 (430, 300), Color.White);
		}
	}
}

