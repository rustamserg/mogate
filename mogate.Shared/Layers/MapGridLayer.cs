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

			int tileSetId = Globals.MAP_TILES_ID[gameState.Level];
			var mapGrid = world.GetLevel (gameState.Level);

			for (int x = 0; x < mapGrid.Width; x++) {
				for (int y = 0; y < mapGrid.Height; y++) {
					var tileEnt = CreateEntity ();
					tileEnt.Register (new Sprite (sprites.GetSprite (string.Format("floor_{0:D2}_01", tileSetId))));
					tileEnt.Register (new Drawable(new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT), 0));
					var id = mapGrid.GetID (x, y);

					if (id == MapGridTypes.ID.Blocked) {
						var ent = CreateEntity ();
						int wallId = Utils.ThrowDice (Globals.MAP_WALLS_MAX[gameState.Level]) + 1;
						ent.Register (new Sprite (sprites.GetSprite (string.Format("wall_{0:D2}_{1:D2}", tileSetId, wallId))));
						ent.Register (new Drawable (new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));
					} else if (id == MapGridTypes.ID.StairDown) {
						var ent = CreateEntity ();
						ent.Register (new Sprite (sprites.GetSprite ("entry_01")));
						ent.Register (new Drawable (new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));
					} else if (id == MapGridTypes.ID.StairUp) {
						if (gameState.Level < Globals.MAX_LEVELS - 1) {
							AddExitPoint (false);
						}
					}
				}
			}
		}

		public void AddExitPoint(bool isFinal)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel (gameState.Level);

			var ent = CreateEntity ();
			ent.Register (new Sprite (sprites.GetSprite (isFinal ? "final_exit_01" : "exit_01")));
			ent.Register (new Drawable (new Vector2 (mapGrid.StairUp.X * Globals.CELL_WIDTH, mapGrid.StairUp.Y * Globals.CELL_HEIGHT)));
			ent.Register (new PointLight (PointLight.DistanceType.Normal, isFinal ? Color.Red : Color.Green));
			ent.Register (new Position (mapGrid.StairUp.X, mapGrid.StairUp.Y));
			ent.Register (new Triggerable((from) => OnExitTriggered(ent, from)));
		}

		void OnExitTriggered(Entity stair, Entity from)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));

			gameState.NextLevel ();
			director.ActivateScene ("inter", TimeSpan.FromSeconds (1));
		}
	}
}

