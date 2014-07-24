using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class InterLayer : Layer
	{
		private SpriteFont m_font;

		public InterLayer (Game game, string name, int z) : base(game, name, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_font = sprites.GetFont ("SpriteFont1");
		}

		protected override void OnPostUpdate(GameTime gameTime)
		{
			if (Keyboard.GetState ().IsKeyDown (Keys.N)) {
				var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
				gameState.NextLevel ();

				var director = (IDirector)Game.Services.GetService (typeof(IDirector));
				var sc = director.GetActiveScene ();
				if (sc.Name != "game") {
					director.ActivateScene ("game", TimeSpan.FromSeconds(2));
				}
			}
		}

		protected override void OnPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.DrawString (m_font, "Press 'N' for next level", new Vector2 (420, 270), Color.White);
		}
	}
}

