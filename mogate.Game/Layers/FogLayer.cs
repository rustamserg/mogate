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

					ent.Get<Drawable> ().DrawAlpha = 0.3f;
				}
			}
		}
	}
}

