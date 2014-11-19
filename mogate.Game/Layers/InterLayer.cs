using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class InterLayer : Layer
	{
		private SpriteFont m_font;

		public InterLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_font = sprites.GetFont ("SpriteFont1");

			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.NextLevel ();
		}

		protected override void OnPostUpdate(GameTime gameTime)
		{
			if (KeyboardUtils.IsKeyPressed(Keys.C)) {
				var director = (IDirector)Game.Services.GetService (typeof(IDirector));
				var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

				if (gameState.IsGameEnd)
					director.ActivateScene ("main");
				else
					director.ActivateScene ("game");
			}
		}

		protected override void OnPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			if (gameState.IsGameEnd)
				spriteBatch.DrawString (m_font, "Press 'C' for new game", new Vector2 (420, 270), Color.White);
			else
				spriteBatch.DrawString (m_font, "Press 'C' for next level", new Vector2 (420, 270), Color.White);
		}
	}
}

