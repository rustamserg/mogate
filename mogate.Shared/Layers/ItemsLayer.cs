using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;


namespace mogate
{
	public enum ConsumableItems { Trap };

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
				ent.Register (new Sprite (sprites.GetSprite ("items_barrel")));
				ent.Register (new Drawable (new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Position (pos.X, pos.Y));
				ent.Register (new Health (1, () => OnBarrelDestroyed(ent)));
				ent.Register (new Attackable ((attacker, _) => OnBarrelAttacked(ent, attacker)));
				ent.Register (new IFFSystem (Globals.IFF_MONSTER_ID));
				ent.Register (new PointLight (4));
			}
		}

		public void AddTrap(Point spawnPoint)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var ent = CreateEntity ();
			ent.Register (new Sprite (sprites.GetSprite ("effects_fire")));
			ent.Register (new Drawable (new Vector2(spawnPoint.X * Globals.CELL_WIDTH, spawnPoint.Y * Globals.CELL_HEIGHT)));
			ent.Register (new Position (spawnPoint.X, spawnPoint.Y));
			ent.Register (new Health (1));
			ent.Register (new Attack (100));
			ent.Register (new Attackable ((attacker, _) => OnTrapAttacked(ent, attacker)));
			ent.Register (new Execute ());
			ent.Register (new IFFSystem (Globals.IFF_PLAYER_ID, 1));
			ent.Register (new PointLight (4));
		}

		void OnTrapAttacked (Entity item, Entity attacker)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			effects.AttachEffect (item, "effects_damage", 200);

			if (attacker.Has<Attackable> ()) {
				var seq = new Sequence ();
				seq.Add (new AttackEntity (item, attacker));
				seq.Add (new ActionEntity (item, OnTrapDestroyed));
				item.Get<Execute> ().Add (seq);
			} else {
				RemoveEntityByTag (item.Tag);	
			}
		}

		void OnTrapDestroyed(Entity trap)
		{
			RemoveEntityByTag (trap.Tag);
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
				ent.Register (new Sprite (sprites.GetSprite ("items_artefact")));
				ent.Register (new Drawable (new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Triggerable (1, (from) => OnArtefactTriggered(ent, from)));
				ent.Register (new Position (mp.X, mp.Y));
				ent.Register (new PointLight (5));
			} else {
				if (Utils.DropChance(Globals.DROP_HEALTH_PROB[gameState.Level])) {
					ent.Register (new Sprite (sprites.GetSprite ("items_life")));
					ent.Register (new Drawable (new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
					ent.Register (new Triggerable (1, (from) => OnHealthTriggered (ent, from)));
				} else if (Utils.DropChance(Globals.DROP_ARMOR_PROB[gameState.Level])) {
					ent.Register (new Sprite (sprites.GetSprite ("items_shield")));
					ent.Register (new Drawable (new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
					ent.Register (new Triggerable (1, (from) => OnArmorTriggered (ent, from)));
				} else {
					ent.Register (new Sprite (sprites.GetSprite ("effects_fire")));
					ent.Register (new Drawable (new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
					ent.Register (new Triggerable (1, (from) => OnTrapTriggered (ent, from)));
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
					from.Get<Armor> ().Value = from.Get<Armor> ().Value + Globals.ARMOR_PACK;
					RemoveEntityByTag (item.Tag);
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

		void OnTrapTriggered (Entity item, Entity from)
		{
			if (from.Has<Consumable<ConsumableItems>> ()) {
				from.Get<Consumable<ConsumableItems>> ().Refill (ConsumableItems.Trap, 1);
				RemoveEntityByTag (item.Tag);
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

