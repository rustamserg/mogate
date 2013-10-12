using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class MonstersLayer : DrawableGameComponent
	{
		SpriteBatch m_spriteBatch;

		public MonstersLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch (Game.GraphicsDevice);
		}

		public override void Draw (GameTime gameTime)
		{
			m_spriteBatch.Begin ();

			var monsters = (IMonsters)Game.Services.GetService(typeof(IMonsters));

			foreach (var pt in monsters.GetMonsters()) {
				m_spriteBatch.Draw(pt.Get<Drawable>().Sprite.Texture,
				                   pt.Get<Drawable>().DrawPos,
				                   pt.Get<Drawable>().DrawRect,
				                   Color.White);
			}

			m_spriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

