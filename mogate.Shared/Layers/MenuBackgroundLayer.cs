using System;
using Microsoft.Xna.Framework;

namespace mogate
{
	public class MenuBackgroundLayer : Layer
	{
		public MenuBackgroundLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			for (int x = 0; x < (Globals.WORLD_WIDTH + 2); x++) {
				for (int y = 0; y < (Globals.WORLD_HEIGHT + 2); y++) {
					var tileEnt = CreateEntity (string.Format("{0}_{1}", x, y));
					tileEnt.Register (new Sprite (sprites.GetSprite ("floor_02_01")));
					tileEnt.Register (new Drawable (new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));
				}
			}
			var controller = CreateEntity ();
			controller.Register (new Execute ());
			controller.Get<Execute> ().Add (new Loop (new ActionEntity (controller, (_) => {
				Update (controller);
			}), 50));
		}

		void Update(Entity controller)
		{
			for (int x = 0; x < (Globals.WORLD_WIDTH + 2); x++) {
				for (int y = 0; y < (Globals.WORLD_HEIGHT + 2); y++) {
					var tileEnt = GetEntityByTag (string.Format("{0}_{1}", x, y));
					tileEnt.Get<Drawable> ().DrawPos.X--;
					tileEnt.Get<Drawable> ().DrawPos.Y--;
					if (tileEnt.Get<Drawable> ().DrawPos.X % Globals.CELL_WIDTH == 0) {
						tileEnt.Get<Drawable> ().DrawPos.X = x * Globals.CELL_WIDTH;
						tileEnt.Get<Drawable> ().DrawPos.Y = y * Globals.CELL_HEIGHT;
					}
				}
			}
		}
	}
}

