using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class EffectsLayer : DrawableGameComponent
	{
		SpriteBatch m_spriteBatch;
		
		public EffectsLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch (Game.GraphicsDevice);
		}

		public override void Draw (GameTime gameTime)
		{
			m_spriteBatch.Begin ();

			var effects = (IEffects)Game.Services.GetService(typeof(IEffects));

			foreach (var pt in effects.GetEffects()) {
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

