using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class HeroLayer : DrawableGameComponent
	{
		SpriteBatch m_spriteBatch;
		Sprite2D m_life;

		public HeroLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_life = sprites.GetSprite ("items_life");
		}

		public override void Draw (GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			if (gameState.State < EState.HeroCreated)
				return;

			m_spriteBatch.Begin();

			var hero = (IHero)Game.Services.GetService (typeof(IHero));
			var ent = hero.Player;

			m_spriteBatch.Draw (ent.Get<Drawable>().Sprite.Texture,
			                    ent.Get<Drawable>().DrawPos,
			                    ent.Get<Drawable>().DrawRect,
			                    Color.White);

			for (int i = 0; i < (int)(hero.Player.Get<Health> ().HP / 20); i++) {
				var drawPos = new Vector2 (i * Globals.CELL_WIDTH, Globals.WORLD_HEIGHT * Globals.CELL_HEIGHT);
				m_spriteBatch.Draw (m_life.Texture, drawPos, m_life.GetFrameRect (0), Color.White);
			}

			m_spriteBatch.End();

			base.Draw (gameTime);
		}
	}
}

