using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;


namespace mogate
{
	public class ItemsLayer : Layer
	{
		public ItemsLayer(Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var mapGrid = world.GetLevel(gameState.Level);

			foreach (var room in mapGrid.GetRooms()) {
				var pos = new Point (room.Pos.X + Utils.Rand.Next (room.Width), room.Pos.Y + Utils.Rand.Next (room.Height));

				var ent = CreateEntity ();
				ent.Register (new Drawable (sprites.GetSprite ("items_barrel"),
					new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Position (pos.X, pos.Y));
				ent.Register (new Health (1, () => OnBarrelDestroyed(ent)));
				ent.Register (new Attackable ((attacker) => OnBarrelAttacked(ent, attacker)));
				ent.Register (new PointLight (4));
			}
		}

		void OnBarrelAttacked (Entity item, Entity attacker)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			effects.AttachEffect (item, "effects_damage", 400);
		}

		void OnBarrelDestroyed(Entity barrel)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			RemoveEntityByTag (barrel.Tag);

			var mp = barrel.Get<Position> ().MapPos;
			var ent = CreateEntity ();

			if (gameState.Level == Globals.MAX_LEVELS - 1) {
				ent.Register (new Drawable (sprites.GetSprite ("items_artefact"),
					new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Triggerable (1, (from) => OnArtefactTriggered(ent, from)));
				ent.Register (new Position (mp.X, mp.Y));
				ent.Register (new PointLight (5));
			} else {
				if (Utils.Rand.Next (100) < 50) {
					ent.Register (new Drawable (sprites.GetSprite ("items_life"),
						new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
					ent.Register (new Triggerable (1, (from) => OnHealthTriggered (ent, from)));
				} else {
					ent.Register (new Drawable (sprites.GetSprite ("items_shield"),
						new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
					ent.Register (new Triggerable (1, (from) => OnArmorTriggered (ent, from)));
				}
				ent.Register (new Position (mp.X, mp.Y));
				ent.Register (new PointLight (3));
			}
			ent.Register (new Position (mp.X, mp.Y));
		}

		void OnArmorTriggered (Entity item, Entity from)
		{
			if (from.Has<Armor> ()) {
				if (from.Get<Armor> ().Value < from.Get<Armor> ().MaxArmor) {
					from.Get<Armor> ().Value = from.Get<Armor> ().Value + 1;
					RemoveEntityByTag (item.Tag);
				}
			}
		}
			
		void OnHealthTriggered (Entity item, Entity from)
		{
			if (from.Has<Health> ()) {
				if (from.Get<Health> ().HP < from.Get<Health> ().MaxHP) {
					from.Get<Health> ().HP = from.Get<Health> ().HP + 1;
					RemoveEntityByTag (item.Tag);
				}
			}
		}

		void OnArtefactTriggered (Entity item, Entity from)
		{
			var mapGridLayer = (MapGridLayer)Scene.GetLayer ("map");

			mapGridLayer.AddExitPoint (true);
			RemoveEntityByTag (item.Tag);
		}
	}
}

