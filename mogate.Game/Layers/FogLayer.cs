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
			var hero = Scene.GetLayer ("hero");
			var items = Scene.GetLayer ("items");
			var maps = Scene.GetLayer ("map");
			var monsters = Scene.GetLayer ("monsters");

			var toLight = hero.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Position> ()).ToList();
			toLight.AddRange (items.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Position> ()));
			toLight.AddRange (maps.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Position> ()));
			toLight.AddRange (monsters.GetAllEntities ().Where (e => e.Has<DirectLight> () && e.Has<Position> ()));

			foreach (var ent in fogent) {
				var fp = ent.Get<Position> ().MapPos;
				ent.Get<Drawable> ().DrawAlpha = 1.0f;

				foreach (var pe in toLight.ToList()) {
					if (pe.Has<PointLight> ()) {
						var lightDist = (float)Utils.Dist (fp, pe.Get<Position> ().MapPos);
						if (lightDist < pe.Get<PointLight> ().Distance) {
							ent.Get<Drawable> ().DrawAlpha *= lightDist / pe.Get<PointLight> ().Distance;
						}
					} else {
						var lightDist = (float)Utils.DirectionDist (pe.Get<Position> ().MapPos, fp, pe.Get<DirectLight> ().Direction);
						if (lightDist >= 0 && lightDist < pe.Get<DirectLight> ().Distance) {
							var id = mapGrid.GetID (fp.X, fp.Y);
							if (id == mogate.MapGridTypes.ID.Blocked) {
								toLight.Remove (pe);
							} else {
								ent.Get<Drawable> ().DrawAlpha *= (pe.Get<DirectLight> ().Distance - lightDist) / (pe.Get<DirectLight> ().Distance + 1);
							}
						}
					}
				}
			}
		}
	}
}

