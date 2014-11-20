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
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			if (gameState.IsLoaded) {
				if (KeyboardUtils.IsKeyPressed (Keys.N)) {
					gameState.NewGame ();
	
					var director = (IDirector)Game.Services.GetService (typeof(IDirector));
					director.ActivateScene ("game");
				} else if (KeyboardUtils.IsKeyPressed (Keys.C)) {
					var director = (IDirector)Game.Services.GetService (typeof(IDirector));
					director.ActivateScene ("game");
				}
			}
		}

		protected override void OnPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			if (gameState.IsLoaded) {
				spriteBatch.DrawString (m_font, "Press 'N' to start your adventure", new Vector2 (420, 270), Color.White);
				if (gameState.Level > 0) {
					spriteBatch.DrawString (m_font, "Press 'C' to continue your adventure", new Vector2 (420, 300), Color.White);
				}
			}
		}
	}
}

