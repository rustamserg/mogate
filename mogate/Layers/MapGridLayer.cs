using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class MapGridLayer: DrawableGameComponent
	{
		Sprite2D m_wall;
		Sprite2D m_tile;
		Sprite2D m_ladder;
		SpriteBatch m_spriteBatch;


		public MapGridLayer (Game game) : base(game)
		{
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_tile = sprites.GetSprite("grid_tile");
			m_wall = sprites.GetSprite("grid_wall");
			m_ladder = sprites.GetSprite("grid_ladder");
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
					Vector2 dest = new Vector2(x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT);

					m_spriteBatch.Draw (m_tile.Texture, dest, m_tile.GetFrameRect (0), Color.White);

					var id = mapGrid.GetID (x, y);
					if (id == MapGridTypes.ID.Blocked)
						m_spriteBatch.Draw (m_wall.Texture, dest, m_wall.GetFrameRect (0), Color.White);
					else if (id == MapGridTypes.ID.StairDown || id == MapGridTypes.ID.StairUp)
						m_spriteBatch.Draw (m_ladder.Texture, dest, m_ladder.GetFrameRect (0), Color.White);
				}
			}

			m_spriteBatch.End ();

			base.Draw(gameTime);
		}
	}
}

