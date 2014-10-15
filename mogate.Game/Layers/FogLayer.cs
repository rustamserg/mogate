using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class FogLayer : Layer
	{
		public FogLayer(Game game, string name, int z) : base(game, name, z)
		{
		}

		public override void OnActivated()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var mapGrid = world.GetLevel(gameState.Level);
			RemoveAllEntities ();

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

			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			var scene = director.GetActiveScene ();

			var hero = scene.GetLayer ("hero");

			var player = hero.GetEntityByTag ("player");
			var pp = player.Get<Position> ().MapPos;
			var dist = player.Get<PointLight> ().Distance;

			foreach (var ent in fogent) {
				var fp = ent.Get<Position> ().MapPos;
				var lightDist = (float)Utils.Dist (fp, pp);
				if (lightDist < player.Get<PointLight> ().Distance) {
					ent.Get<Drawable> ().DrawAlpha = lightDist / player.Get<PointLight> ().Distance;
				} else {
					ent.Get<Drawable> ().DrawAlpha = 1.0f;
				}
			}
		}
	}
}

