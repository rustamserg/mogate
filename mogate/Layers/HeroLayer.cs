using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class HeroLayer : DrawableGameComponent
	{
		Texture2D m_hero;
		Texture2D m_life;
		SpriteBatch m_spriteBatch;

		public HeroLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			m_hero = Game.Content.Load<Texture2D>("hero");
			m_life = Game.Content.Load<Texture2D> ("life");
		}

		public override void Draw (GameTime gameTime)
		{
			m_spriteBatch.Begin();

			var hero = (IHero)Game.Services.GetService (typeof(IHero));

			if (hero.Player.Get<Health>().HP < hero.Player.Get<Health>().MaxHP/2)
				m_spriteBatch.Draw (m_hero, hero.Player.Get<Position>().DrawPos, Color.Red);
			else
				m_spriteBatch.Draw (m_hero, hero.Player.Get<Position>().DrawPos, Color.White);

			for (int i = 0; i < (int)(hero.Player.Get<Health>().HP / 20); i++)
				m_spriteBatch.Draw (m_life, new Vector2 (i * 32, 23 * 32), Color.White);

			m_spriteBatch.End();

			base.Draw (gameTime);
		}
	}
}

