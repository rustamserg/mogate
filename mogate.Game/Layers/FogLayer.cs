using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace mogate
{
	public class FogLayer : Layer
	{
		public FogLayer(Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var mapGrid = world.GetLevel(gameState.Level);

			for (int w = 0; w < mapGrid.Width; w++) {
				for (int h = 0; h < mapGrid.Height; h++) {
					var ent = CreateEntity ();
					ent.Register(new Drawable(sprites.GetSprite("effects_fog"),
						new Vector2(w * Globals.CELL_WIDTH, h * Globals.CELL_HEIGHT)));
					ent.Register (new Position (w, h));
				}
			}
		}

		protected override void OnPostUpdate(GameTime gameTime)
		{
			var fogent = GetAllEntities ();

			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel (gameState.Level);
			var hero = Scene.GetLayer ("player");
			var items = Scene.GetLayer ("items");
			var maps = Scene.GetLayer ("map");

			var toLight = hero.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Position> ()).ToList();
			toLight.AddRange (items.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Position> ()));
			toLight.AddRange (maps.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Position> ()));

			foreach (var ent in fogent) {
				var fp = ent.Get<Position> ().MapPos;
				ent.Get<Drawable> ().DrawAlpha = 1.0f;

				foreach (var pe in toLight) {
					var lightDist = (float)Utils.Dist (fp, pe.Get<Position> ().MapPos);
					if (lightDist < pe.Get<PointLight> ().Distance) {
						ent.Get<Drawable> ().DrawAlpha *= lightDist / pe.Get<PointLight> ().Distance;
					}
				}
			}
		}
	}
}

