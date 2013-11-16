using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class ItemsLayer : DrawableGameComponent
	{
		Texture2D m_chest;
		SpriteBatch m_spriteBatch;


		public ItemsLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch (Game.GraphicsDevice);

			m_chest = Game.Content.Load<Texture2D> ("bochka");
		}

		public override void Update (GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			if (gameState.State == EState.HeroCreated) {
				var items = (IItems)Game.Services.GetService(typeof(IItems));
				items.Init();
	
				gameState.State = EState.ItemsCreated;
			}

			base.Update(gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			m_spriteBatch.Begin ();

			var items = (IItems)Game.Services.GetService(typeof(IItems));

			foreach (var pt in items.GetItems()) {
				Vector2 drawPos = new Vector2(pt.X * Globals.CELL_WIDTH, pt.Y * Globals.CELL_HEIGHT);
				m_spriteBatch.Draw(m_chest, drawPos, Color.White);
			}

			m_spriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

