using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class MapGridLayer: DrawableGameComponent
	{
		Texture2D m_wall;
		Texture2D m_tile;
		Texture2D m_ladder;
		SpriteBatch m_spriteBatch;


		public MapGridLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			m_tile = Game.Content.Load<Texture2D>("tile");
			m_wall = Game.Content.Load<Texture2D>("wall");
			m_ladder = Game.Content.Load<Texture2D>("ladder");
		}

		public override void Update (GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			if (gameState.State == EState.WorldLoaded) {
				gameState.State = EState.LevelCreated;
			}
		}

		public override void Draw (GameTime gameTime)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			if (gameState.State < EState.LevelCreated)
				return;

			var mapGrid = world.GetLevel (gameState.Level);

			m_spriteBatch.Begin ();

			for (int x = 0; x < mapGrid.Width; x++) {
				for (int y = 0; y < mapGrid.Height; y++) {
					Vector2 dest = new Vector2(x * 32, y * 32);

					m_spriteBatch.Draw (m_tile, dest, Color.White);

					var id = mapGrid.GetID (x, y);
					if (id == MapGridTypes.ID.Blocked)
						m_spriteBatch.Draw (m_wall, dest, Color.White);
					else if (id == MapGridTypes.ID.StairDown || id == MapGridTypes.ID.StairUp)
						m_spriteBatch.Draw (m_ladder, dest, Color.White);
				}
			}

			m_spriteBatch.End ();

			base.Draw(gameTime);
		}
	}
}

