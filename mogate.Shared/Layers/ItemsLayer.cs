using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;


namespace mogate
{
	public enum ConsumableTypes { Money };
	public enum LootTypes { Money, Health, Antitod, Armor, Weapon };

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
				ent.Register (new Health (1, () => OnChestDestroyed(ent)));
				ent.Register (new Attackable ((attacker, _) => OnChestAttacked(ent, attacker)));
				ent.Register (new IFFSystem (Globals.IFF_MONSTER_ID));
				ent.Register (new PointLight (PointLight.DistanceType.Small, Color.Green));
			}
		}

		public void DropLoot(Point pos, Dictionary<string, int>[] loots)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			loots.Shuffle ();
			int maxWeight = Globals.DROP_LOOT_WEIGHT [gameState.Level];

			foreach (var arch in loots) {
				var w = Utils.ThrowDice (maxWeight);
				var lootType = (LootTypes)arch ["loot_type"];

				if (arch ["spawn_weight"] >= w && arch["spawn_weight"] <= maxWeight) {
					var ent = CreateEntity ();
					ent.Register (new Drawable (new Vector2 (pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
					ent.Register (new Position (pos.X, pos.Y));
					ent.Register (new PointLight (PointLight.DistanceType.Small, Color.Gold));
					ent.Register (new State<LootTypes> (lootType));

					if (lootType == LootTypes.Money) {
						ent.Register (new Sprite (sprites.GetSprite ("money_01")));
						ent.Register (new Triggerable (1, (from) => OnMoneyTriggered (ent, from)));
						ent.Register (new Loot (Utils.ThrowDice (arch ["money"])));
					} else if (lootType == LootTypes.Health) {
						ent.Register (new Sprite (sprites.GetSprite ("health_potion_01")));
						ent.Register (new Triggerable (1, (from) => OnHealthTriggered (ent, from)));
						ent.Register (new Loot (Utils.ThrowDice (arch ["health"])));
					} else if (lootType == LootTypes.Armor) {
						var armorSprite = string.Format ("armor_{0:D2}", Archetypes.Armors [arch ["armor_index"]] ["sprite_index"]);
						ent.Register (new Sprite (sprites.GetSprite (armorSprite)));
						ent.Register (new Triggerable (1, (from) => OnArmorTriggered (ent, from)));
						ent.Register (new Loot (arch ["armor_index"]));
						ent.Register (new Price (arch ["price"]));
					} else if (lootType == LootTypes.Weapon) {
						var weaponSprite = string.Format ("weapon_{0:D2}", Archetypes.Weapons [arch ["weapon_index"]] ["sprite_index"]);
						ent.Register (new Sprite (sprites.GetSprite (weaponSprite)));
						ent.Register (new Triggerable (1, (from) => OnWeaponTriggered (ent, from)));
						ent.Register (new Loot (arch ["weapon_index"]));
						ent.Register (new Price (arch ["price"]));
					} else {
						ent.Register (new Sprite (sprites.GetSprite ("antitod_potion_01")));
						ent.Register (new Triggerable (1, (from) => OnAntitodTriggered (ent, from)));
					}
					break;
				}
			}
		}

		void OnArmorTriggered(Entity item, Entity attacker)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var hud = (HUDLayer)Scene.GetLayer ("hud");

			if (attacker.Has<Consumable<ConsumableTypes>> ()) {
				int cost = item.Get<Price> ().Cost;
				int armorId = item.Get<Loot> ().Drop;

				if (attacker.Has<Armor> () && attacker.Get<Armor> ().ArchetypeID >= armorId) {
					hud.FeedbackMessage ("Broken");
				} else {
					if (attacker.Get<Consumable<ConsumableTypes>> ().TryConsume (ConsumableTypes.Money, cost)) {
						if (!attacker.Has<Armor> ()) {
							attacker.Register (new Armor (Archetypes.Armors [armorId] ["defence"], armorId));
						} else {
							attacker.Get<Armor> ().ArchetypeID = armorId;
							attacker.Get<Armor> ().Defence = Archetypes.Armors [armorId] ["defence"];
						}
						gameState.PlayerArmorID = armorId;
						RemoveEntityByTag (item.Tag);
					} else {
						string feedbackMsg = string.Format ("Cost: {0}", cost);
						hud.FeedbackMessage (feedbackMsg);
					}
				}
			}
		}

		void OnWeaponTriggered(Entity item, Entity attacker)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var hud = (HUDLayer)Scene.GetLayer ("hud");

			if (attacker.Has<Consumable<ConsumableTypes>> ()) {
				int cost = item.Get<Price> ().Cost;
				int weaponId = item.Get<Loot> ().Drop;

				if (attacker.Has<Attack> () && attacker.Get<Attack> ().ArchetypeID >= weaponId) {
					hud.FeedbackMessage ("Broken");
				} else {
					if (attacker.Get<Consumable<ConsumableTypes>> ().TryConsume (ConsumableTypes.Money, cost)) {
						attacker.Get<Attack> ().ArchetypeID = weaponId;
						attacker.Get<Attack> ().Damage = Archetypes.Weapons [weaponId] ["attack"];
						gameState.PlayerWeaponID = weaponId;
						RemoveEntityByTag (item.Tag);
					} else {
						string feedbackMsg = string.Format ("Cost: {0}", cost);
						hud.FeedbackMessage (feedbackMsg);
					}
				}
			}
		}

		void OnMoneyTriggered(Entity item, Entity attacker)
		{
			if (attacker.Has<Consumable<ConsumableTypes>> ()) {
				int droppedMoney = item.Get<Loot>().Drop;
				attacker.Get<Consumable<ConsumableTypes>> ().Refill (ConsumableTypes.Money, droppedMoney);

				RemoveEntityByTag (item.Tag);
			}
		}

		void OnChestAttacked (Entity item, Entity attacker)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			effects.AttachEffect (item, "damage_01", 400);
		}

		void OnChestDestroyed(Entity chest)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			RemoveEntityByTag (chest.Tag);
			var mp = chest.Get<Position> ().MapPos;

			if (gameState.Level == Globals.MAX_LEVELS - 1) {
				var ent = CreateEntity ();

				ent.Register (new Drawable (new Vector2 (mp.X * Globals.CELL_WIDTH, mp.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Position (mp.X, mp.Y));
				ent.Register (new PointLight (PointLight.DistanceType.Normal, Color.Green));
				ent.Register (new Sprite (sprites.GetSprite ("artefact_01")));
				ent.Register (new Triggerable (1, (from) => OnArtefactTriggered(ent, from)));
			} else {
				DropLoot (mp, Archetypes.ChestLoot);
			}
		}
			
		void OnHealthTriggered (Entity item, Entity from)
		{
			if (from.Has<Health> ()) {
				if (from.Get<Health> ().HP < from.Get<Health> ().MaxHP) {
					from.Get<Health> ().HP = from.Get<Health> ().HP + item.Get<Loot>().Drop;
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

