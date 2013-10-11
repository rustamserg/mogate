using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class HeroLayer : DrawableGameComponent
	{
		Texture2D m_life;
		SpriteBatch m_spriteBatch;

		public HeroLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
			m_life = Game.Content.Load<Texture2D> ("life");
		}

		public override void Draw (GameTime gameTime)
		{
			m_spriteBatch.Begin();

			var hero = (IHero)Game.Services.GetService (typeof(IHero));
			var ent = hero.Player;

			m_spriteBatch.Draw (ent.Get<Drawable>().Sprite.Texture,
			                    ent.Get<Drawable>().DrawPos,
			                    ent.Get<Drawable>().Sprite.GetFrame(ent.Get<Drawable>().FrameId),
			                    Color.White);

			for (int i = 0; i < (int)(hero.Player.Get<Health>().HP / 20); i++)
				m_spriteBatch.Draw (m_life, new Vector2 (i * 32, 23 * 32), Color.White);

			m_spriteBatch.End();

			base.Draw (gameTime);
		}
	}
}

