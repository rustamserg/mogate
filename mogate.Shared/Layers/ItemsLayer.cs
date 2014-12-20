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
				ent.Register (new Sprite (sprites.GetSprite ("skeleton_01")));
				ent.Register (new Drawable (new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Position (pos.X, pos.Y));
				ent.Register (new Health (1, () => OnBarrelDestroyed(ent)));
				ent.Register (new Attackable ((attacker, _) => OnBarrelAttacked(ent, attacker)));
				ent.Register (new IFFSystem (Globals.IFF_MONSTER_ID));
				ent.Register (new PointLight (4));
			}
		}

		void OnBarrelAttacked (Entity item, Entity attacker)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			effects.AttachEffect (item, "damage_01", 400);
		}

		void OnBarrelDestroyed(Entity barrel)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			RemoveEntityByTag (barrel.Tag);

			var mp = barrel.Get<Position> ().MapPos;
			var ent = CreateEntity ();

			ent.Register (new Drawable (new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
			ent.Register (new Position (mp.X, mp.Y));
			ent.Register (new PointLight (5));

			if (gameState.Level == Globals.MAX_LEVELS - 1) {
				ent.Register (new Sprite (sprites.GetSprite ("artefact_01")));
				ent.Register (new Triggerable (1, (from) => OnArtefactTriggered(ent, from)));
			} else {
				if (Utils.DropChance(Globals.DROP_HEALTH_PROB[gameState.Level])) {
					ent.Register (new Sprite (sprites.GetSprite ("health_potion_01")));
					ent.Register (new Triggerable (1, (from) => OnHealthTriggered (ent, from)));
				} else {
					ent.Register (new Sprite (sprites.GetSprite ("antitod_potion_01")));
					ent.Register (new Triggerable (1, (from) => OnAntitodTriggered (ent, from)));
				}
			}
		}
			
		void OnHealthTriggered (Entity item, Entity from)
		{
			if (from.Has<Health> ()) {
				if (from.Get<Health> ().HP < from.Get<Health> ().MaxHP) {
					from.Get<Health> ().HP = from.Get<Health> ().HP + Globals.HEALTH_PACK;
					RemoveEntityByTag (item.Tag);
				}
			}
		}

		void OnAntitodTriggered (Entity item, Entity from)
		{
			if (from.Has<Poisonable> ()) {
				if (from.Get<Poisonable> ().IsPoisoned) {
					from.Get<Poisonable> ().CancelPoison (from);
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

