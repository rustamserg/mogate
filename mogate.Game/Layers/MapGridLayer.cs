using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class MapGridLayer: Layer
	{
		Sprite2D m_wall;
		Sprite2D m_tile;
		Sprite2D m_ladder;


		public MapGridLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_tile = sprites.GetSprite("grid_tile");
			m_wall = sprites.GetSprite("grid_wall");
			m_ladder = sprites.GetSprite("grid_ladder");
		}

		protected override void OnPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel (gameState.Level);

			for (int x = 0; x < mapGrid.Width; x++) {
				for (int y = 0; y < mapGrid.Height; y++) {
					Vector2 dest = new Vector2(x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT);

					spriteBatch.Draw (m_tile.Texture, dest, m_tile.GetFrameRect (0), Color.White);

					var id = mapGrid.GetID (x, y);
					if (id == MapGridTypes.ID.Blocked)
						spriteBatch.Draw (m_wall.Texture, dest, m_wall.GetFrameRect (0), Color.White);
					else if (id == MapGridTypes.ID.StairDown || id == MapGridTypes.ID.StairUp)
						spriteBatch.Draw (m_ladder.Texture, dest, m_ladder.GetFrameRect (0), Color.White);
				}
			}
		}
	}
}

