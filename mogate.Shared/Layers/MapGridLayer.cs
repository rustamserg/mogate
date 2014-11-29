using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public class MapGridLayer: Layer
	{
		public MapGridLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel (gameState.Level);

			for (int x = 0; x < mapGrid.Width; x++) {
				for (int y = 0; y < mapGrid.Height; y++) {
					var tileEnt = CreateEntity ();
					tileEnt.Register (new Sprite (sprites.GetSprite ("grid_tile")));
					tileEnt.Register (new Drawable(new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT), 0));
					var id = mapGrid.GetID (x, y);

					if (id == MapGridTypes.ID.Blocked) {
						var ent = CreateEntity ();
						ent.Register (new Sprite (sprites.GetSprite ("grid_wall")));
						ent.Register (new Drawable (new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));
					} else if (id == MapGridTypes.ID.StairDown) {
						var ent = CreateEntity ();
						ent.Register (new Sprite (sprites.GetSprite ("grid_ladder")));
						ent.Register (new Drawable (new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));
					} else if (id == MapGridTypes.ID.StairUp) {
						if (gameState.Level < Globals.MAX_LEVELS - 1) {
							AddExitPoint (false);
						}
					}
				}
			}
		}

		public void AddExitPoint(bool lightExit)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel (gameState.Level);

			var ent = CreateEntity ();
			ent.Register (new Sprite (sprites.GetSprite ("grid_ladder")));
			ent.Register (new Drawable (new Vector2 (mapGrid.StairUp.X * Globals.CELL_WIDTH, mapGrid.StairUp.Y * Globals.CELL_HEIGHT)));
			if (lightExit) {
				ent.Register (new PointLight (5));
			}
			ent.Register (new Position (mapGrid.StairUp.X, mapGrid.StairUp.Y));
			ent.Register (new Triggerable(1, (from) => OnExitTriggered(ent, from)));
		}

		void OnExitTriggered(Entity stair, Entity from)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			director.ActivateScene ("inter", TimeSpan.FromSeconds (1));
		}
	}
}

