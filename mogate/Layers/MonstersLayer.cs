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
			var monsters = (IMonsters)Game.Services.GetService(typeof(IMonsters));

			if (gameState.State == EState.ItemsCreated) {
				monsters.Init();
				gameState.State = EState.MonstersCreated;
			}

			if (gameState.State == EState.LevelStarted) {
				foreach (var pt in monsters.GetMonsters()) {
					monsters.UpdateMapPos (pt);
				}
				monsters.UpdateActions ();
			}

			base.Update(gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			m_spriteBatch.Begin ();

			var monsters = (IMonsters)Game.Services.GetService(typeof(IMonsters));

			foreach (var pt in monsters.GetMonsters()) {
				monsters.UpdateDrawPos (pt);
				m_spriteBatch.Draw(m_monster, pt.Get<Position>().DrawPos, Color.White);
			}

			m_spriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

