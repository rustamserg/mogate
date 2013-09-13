using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class MonstersLayer : DrawableGameComponent
	{
		Texture2D m_monster;
		SpriteBatch m_spriteBatch;

		public MonstersLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch (Game.GraphicsDevice);

			m_monster = Game.Content.Load<Texture2D> ("monster");
		}

		public override void Update (GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			if (gameState.State == EState.ItemsCreated) {
				var monsters = (IMonsters)Game.Services.GetService(typeof(IMonsters));
				monsters.Init();

				gameState.State = EState.MonstersCreated;
			}

			if (gameState.State == EState.LevelStarted) {
				var monsters = (IMonsters)Game.Services.GetService (typeof(IMonsters));
				foreach (var pt in monsters.GetMonsters()) {
					monsters.TryChaise (pt);
				}
			}

			base.Update(gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			m_spriteBatch.Begin ();

			var monsters = (IMonsters)Game.Services.GetService(typeof(IMonsters));
			monsters.UpdateDrawPos ();

			foreach (var pt in monsters.GetMonsters()) {
				m_spriteBatch.Draw(m_monster, pt.DrawPos, Color.White);
			}

			m_spriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

