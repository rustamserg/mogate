using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class ItemsLayer : DrawableGameComponent
	{
		SpriteBatch m_spriteBatch;
		Sprite2D m_barrel;


		public ItemsLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch (Game.GraphicsDevice);

			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_barrel = sprites.GetSprite ("items_barrel");
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
				var drawPos = new Vector2(pt.X * Globals.CELL_WIDTH, pt.Y * Globals.CELL_HEIGHT);
				m_spriteBatch.Draw(m_barrel.Texture, drawPos, m_barrel.GetFrameRect(0), Color.White);
			}

			m_spriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

