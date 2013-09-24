using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class HeroLayer : DrawableGameComponent
	{
		Texture2D m_hero;
		SpriteBatch m_spriteBatch;

		public HeroLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			m_hero = Game.Content.Load<Texture2D>("hero");
		}

		public override void Draw (GameTime gameTime)
		{
			m_spriteBatch.Begin();

			var hero = (IHero)Game.Services.GetService (typeof(IHero));

			m_spriteBatch.Draw (m_hero, hero.Player.Get<Position>().DrawPos, Color.White);
			m_spriteBatch.End();

			base.Draw (gameTime);
		}
	}
}

