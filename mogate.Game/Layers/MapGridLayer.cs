using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class MapGridLayer: Layer
	{
		Sprite2D m_tile;

		public MapGridLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel (gameState.Level);
			m_tile = sprites.GetSprite("grid_tile");

			for (int x = 0; x < mapGrid.Width; x++) {
				for (int y = 0; y < mapGrid.Height; y++) {

					var id = mapGrid.GetID (x, y);

					if (id == MapGridTypes.ID.Blocked) {
						var ent = CreateEntity ();
						ent.Register (new Drawable (sprites.GetSprite ("grid_wall"),
							new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));
					} if (id == MapGridTypes.ID.StairDown || id == MapGridTypes.ID.StairUp) {
						var ent = CreateEntity ();
						ent.Register (new Drawable (sprites.GetSprite ("grid_ladder"),
							new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));
					}
				}
			}
		}

		protected override void OnPreDraw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel (gameState.Level);

			for (int x = 0; x < mapGrid.Width; x++) {
				for (int y = 0; y < mapGrid.Height; y++) {
					Vector2 dest = new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT);
					spriteBatch.Draw (m_tile.Texture, dest, m_tile.GetFrameRect (0), Color.White);
				}
			}
		}
	}
}

