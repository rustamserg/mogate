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

			var hero = (IHero)Game.Services.GetService (typeof(IHero));

			m_spriteBatch.Draw (m_hero, hero.Player.Get<Position>().DrawPos, Color.White);
			m_spriteBatch.End();

			base.Draw (gameTime);
		}

		private void UpdateHero (GameTime gameTime)
		{
			var hero = (IHero)Game.Services.GetService (typeof(IHero));
			var mapGrid = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));

			if (hero.Player.Get<Position>().DrawPos.X == hero.Player.Get<Position>().MapPos.X * 32
			    && hero.Player.Get<Position>().DrawPos.Y == hero.Player.Get<Position>().MapPos.Y * 32) {
				Point newPos = hero.Player.Get<Position>().MapPos;

				// check for monsters
				bool isBounce = false;
				var monsters = (IMonsters)Game.Services.GetService (typeof(IMonsters));
				foreach (var mp in monsters.GetMonsters()) {
					if (mp.Get<Position>().MapPos == hero.Player.Get<Position>().MapPos) {
						newPos = m_prevPos;
						isBounce = true;

						mp.Get<ActionQueue> ().Add (new AttackEntity (mp, 50));

						break;
					}
				}

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
					m_prevPos = hero.Player.Get<Position>().MapPos;
					hero.Player.Get<Position>().MapPos = newPos;

					if (mt == MapGridTypes.ID.StairUp) {
						var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
						gameState.State = EState.LevelStarting;
					}
				}
			} else {
				if (hero.Player.Get<Position>().DrawPos.X < hero.Player.Get<Position>().MapPos.X * 32)
					hero.Player.Get<Position>().DrawPos.X += 4;
				if (hero.Player.Get<Position>().DrawPos.X > hero.Player.Get<Position>().MapPos.X * 32)
					hero.Player.Get<Position>().DrawPos.X -= 4;
				if (hero.Player.Get<Position>().DrawPos.Y < hero.Player.Get<Position>().MapPos.Y * 32)
					hero.Player.Get<Position>().DrawPos.Y += 4;
				if (hero.Player.Get<Position>().DrawPos.Y > hero.Player.Get<Position>().MapPos.Y * 32)
					hero.Player.Get<Position>().DrawPos.Y -= 4;
			}
		}

		private void CreateHero()
		{
			var hero = (IHero)Game.Services.GetService(typeof(IHero));
			hero.Init();
			m_prevPos = hero.Player.Get<Position>().MapPos;
		}
	}
}

