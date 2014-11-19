using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mogate
{
	public class MenuLayer : Layer
	{
		private SpriteFont m_font;

		public MenuLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_font = sprites.GetFont ("SpriteFont1");
		}

		protected override void OnPostUpdate(GameTime gameTime)
		{
			if (KeyboardUtils.IsKeyPressed(Keys.N)) {
				var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
				gameState.NewGame ();
	
				var director = (IDirector)Game.Services.GetService (typeof(IDirector));
				director.ActivateScene ("game");
			}
		}

		protected override void OnPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.DrawString (m_font, "Press 'N' to start your adventure", new Vector2 (420, 270), Color.White);
		}
	}
}

