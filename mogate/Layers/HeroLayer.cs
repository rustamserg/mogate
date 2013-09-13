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
		Vector2 m_heroDrawPos;
		Point m_prevPos;

		public HeroLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			m_hero = Game.Content.Load<Texture2D>("hero");
		}

		public override void Update (GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			if (gameState.State == EState.MapGenerated) {
				CreateHero();
				gameState.State = EState.HeroCreated;
			}

			if (gameState.State == EState.LevelStarted) {
				UpdateHero(gameTime);
			}
			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			m_spriteBatch.Begin();
			m_spriteBatch.Draw (m_hero, m_heroDrawPos, Color.White);
			m_spriteBatch.End();

			base.Draw (gameTime);
		}

		private void UpdateHero (GameTime gameTime)
		{
			var hero = (IHero)Game.Services.GetService (typeof(IHero));
			var mapGrid = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));

			if (m_heroDrawPos.X == hero.Position.X * 32 && m_heroDrawPos.Y == hero.Position.Y * 32) {
				Point newPos = hero.Position;

				// check for monsters
				bool isBounce = false;
				/*var monsters = (IMonsters)Game.Services.GetService (typeof(IMonsters));
				foreach (var mp in monsters.GetMonsters()) {
					if (mp.MapPos == hero.Position) {
						newPos = m_prevPos;
						isBounce = true;
						break;
					}
				}*/

				if (!isBounce) {
					if (Keyboard.GetState ().IsKeyDown (Keys.Up))
						newPos.Y--;
					else if (Keyboard.GetState ().IsKeyDown (Keys.Down))
						newPos.Y++;
					else if (Keyboard.GetState ().IsKeyDown (Keys.Left))
						newPos.X--;
					else if (Keyboard.GetState ().IsKeyDown (Keys.Right))
						newPos.X++;
				}

				var mt = mapGrid.GetID (newPos.X, newPos.Y);
				if (mt != MapGridTypes.ID.Blocked) {
					m_prevPos = hero.Position;
					hero.Position = newPos;

					if (mt == MapGridTypes.ID.StairUp) {
						var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
						gameState.State = EState.LevelStarting;
					}
				}
			} else {
				if (m_heroDrawPos.X < hero.Position.X * 32)
					m_heroDrawPos.X += 4;
				if (m_heroDrawPos.X > hero.Position.X * 32)
					m_heroDrawPos.X -= 4;
				if (m_heroDrawPos.Y < hero.Position.Y * 32)
					m_heroDrawPos.Y += 4;
				if (m_heroDrawPos.Y > hero.Position.Y * 32)
					m_heroDrawPos.Y -= 4;
			}
		}

		private void CreateHero()
		{
			var hero = (IHero)Game.Services.GetService(typeof(IHero));
			hero.Init();

			m_heroDrawPos.X = hero.Position.X * 32;
			m_heroDrawPos.Y = hero.Position.Y * 32;
			m_prevPos = hero.Position;
		}
	}
}

