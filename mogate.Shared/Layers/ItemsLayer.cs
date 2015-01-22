using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Elizabeth;


namespace mogate
{
	public enum ConsumableTypes { Money, Antitod };
	public enum LootTypes { Money, Health, Antitod, Armor, Weapon };
	public enum TreasureTypes { Chest, Trash };

	public class ItemsLayer : Layer
	{
		public ItemsLayer(Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var spawner = CreateEntity ();
			int spawnDelay = Globals.TRASH_SPAWN_DELAY_MSEC [gameState.Level];
			spawner.Register (new Execute ());
			spawner.Get<Execute> ().Add (new Loop (new ActionEntity (spawner, SpawnTrash), spawnDelay));

			spawner = CreateEntity ();
			spawnDelay = Globals.CHEST_SPAWN_DELAY_SEC [gameState.Level] * 1000;
			spawner.Register (new Execute ());

			var seq = new Sequence ();
			seq.Add (new ActionEntity (spawner, SpawnChest));
			seq.Add (new Loop (new ActionEntity (spawner, SpawnChest), spawnDelay));
			spawner.Get<Execute> ().Add (seq);
		}

		public void DropLoot(Point pos, Dictionary<string, int>[] loots, int maxWeight)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			loots.Shuffle ();

			foreach (var arch in loots) {
				var w = Utils.ThrowDice (maxWeight);
				var lootType = (LootTypes)arch ["loot_type"];

				if (arch ["spawn_weight"] >= w && arch["spawn_weight"] <= maxWeight) {
					var ent = CreateEntity ();
					ent.Register (new Drawable (new Vector2 (pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
					ent.Register (new Position (pos.X, pos.Y));
					ent.Register (new PointLight (PointLight.DistanceType.Small, Color.Gold));
					ent.Register (new State<LootTypes> (lootType));
					ent.Register (new Execute ());

					var seq = new Sequence ();
					seq.Add (new Delay (arch ["lifetime_sec"] * 1000));
					seq.Add (new ActionEntity (ent, (_) => {
						var blink = new Loop(new ActionEntity(ent, (__) => {
							if (ent.Get<Drawable>().DrawAlpha == 0.0f) {
								ent.Get<Drawable>().DrawAlpha = 1.0f;
							} else {
								ent.Get<Drawable>().DrawAlpha = 0.0f;
							}
						}), 250);
						ent.Get<Execute>().Add(blink, "blink_loop");
					}));
					seq.Add (new Delay (arch ["lifetime_sec"] * 500));
					seq.Add (new ActionEntity (ent, (_) => {
						RemoveEntityByTag(ent.Tag);
					}));
					ent.Get<Execute> ().Add (seq, "lifetime");

					if (lootType == LootTypes.Money) {
						ent.Register (new Sprite (sprites.GetSprite ("money_01")));
						ent.Register (new Triggerable ((from) => OnMoneyTriggered (ent, from)));
						ent.Register (new Loot (arch["money"]));
					} else if (lootType == LootTypes.Health) {
						ent.Register (new Sprite (sprites.GetSprite ("health_potion_01")));
						ent.Register (new Triggerable ((from) => OnHealthTriggered (ent, from)));
						ent.Register (new Loot (arch ["health"]));
					} else if (lootType == LootTypes.Armor) {
						var armorSprite = string.Format ("armor_{0:D2}", Archetypes.Armors [arch ["armor_index"]] ["sprite_index"]);
						ent.Register (new Sprite (sprites.GetSprite (armorSprite)));
						ent.Register (new Triggerable ((from) => OnArmorTriggered (ent, from)));
						ent.Register (new Loot (arch ["armor_index"]));
						ent.Register (new Price (arch ["price"]));
					} else if (lootType == LootTypes.Weapon) {
						var weaponSprite = string.Format ("weapon_{0:D2}", Archetypes.Weapons [arch ["weapon_index"]] ["sprite_index"]);
						ent.Register (new Sprite (sprites.GetSprite (weaponSprite)));
						ent.Register (new Triggerable ((from) => OnWeaponTriggered (ent, from)));
						ent.Register (new Loot (arch ["weapon_index"]));
						ent.Register (new Price (arch ["price"]));
					} else {
						ent.Register (new Sprite (sprites.GetSprite ("antitod_potion_01")));
						ent.Register (new Triggerable ((from) => OnAntitodTriggered (ent, from)));
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
					hud.FeedbackMessage ("Already weared", Color.Red);
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
						hud.FeedbackMessage ("Picked up new armor", Color.Yellow);
					} else {
						string feedbackMsg = string.Format ("Need {0} money", cost);
						hud.FeedbackMessage (feedbackMsg, Color.Red);
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
					hud.FeedbackMessage ("Already weared", Color.Red);
				} else {
					if (attacker.Get<Consumable<ConsumableTypes>> ().TryConsume (ConsumableTypes.Money, cost)) {
						attacker.Get<Attack> ().ArchetypeID = weaponId;
						attacker.Get<Attack> ().Damage = Archetypes.Weapons [weaponId] ["attack"];
						attacker.Get<CriticalHit> ().HitChance = Archetypes.Weapons [weaponId] ["critical_chance"];
						attacker.Get<CriticalHit> ().CriticalDamage = Archetypes.Weapons [weaponId] ["critical_damage"];
						gameState.PlayerWeaponID = weaponId;

						RemoveEntityByTag (item.Tag);
						hud.FeedbackMessage ("Picked up new weapon", Color.Yellow);
					} else {
						string feedbackMsg = string.Format ("Need {0} money", cost);
						hud.FeedbackMessage (feedbackMsg, Color.Red);
					}
				}
			}
		}

		void OnMoneyTriggered(Entity item, Entity attacker)
		{
			var hud = (HUDLayer)Scene.GetLayer ("hud");

			if (attacker.Has<Consumable<ConsumableTypes>> ()) {
				int droppedMoney = item.Get<Loot>().Drop;
				float moneyMult = attacker.Get<MoneyMultiplier> ().Multiplier;
				droppedMoney = (int)(droppedMoney * moneyMult);
				attacker.Get<Consumable<ConsumableTypes>> ().Refill (ConsumableTypes.Money, droppedMoney);
				RemoveEntityByTag (item.Tag);

				string feedbackMsg = string.Format ("Picked {0} money", droppedMoney);
				hud.FeedbackMessage (feedbackMsg, Color.Yellow);
			}
		}

		void OnChestAttacked (Entity item, Entity attacker)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			effects.AttachEffect (item, "damage_01", 400);
		}

		void OnChestDestroyed(Entity chest)
		{
			if (chest.Get<Health> ().HP > 0)
				return;

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
				ent.Register (new Triggerable ((from) => OnArtefactTriggered(ent, from)));
			} else {
				DropLoot (mp, Archetypes.ChestLoot, Globals.CHEST_DROP_LOOT_WEIGHT[gameState.Level]);
			}
		}

		void OnTrashDestroyed(Entity trash)
		{
			if (trash.Get<Health> ().HP > 0)
				return;

			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			RemoveEntityByTag (trash.Tag);
			var mp = trash.Get<Position> ().MapPos;
			DropLoot (mp, Archetypes.TrashLoot, Globals.TRASH_DROP_LOOT_WEIGHT[gameState.Level]);
		}

		void OnHealthTriggered (Entity item, Entity from)
		{
			var hud = (HUDLayer)Scene.GetLayer ("hud");

			if (from.Has<Health> ()) {
				if (from.Get<Health> ().HP < from.Get<Health> ().MaxHP) {
					from.Get<Health> ().HP = from.Get<Health> ().HP + item.Get<Loot>().Drop;
					RemoveEntityByTag (item.Tag);
					hud.FeedbackMessage ("Picked up health potion", Color.Yellow);
				}
			}
		}

		void OnAntitodTriggered (Entity item, Entity from)
		{
			var hud = (HUDLayer)Scene.GetLayer ("hud");

			if (from.Has<Poisonable> ()) {
				if (from.Get<Poisonable> ().IsPoisoned) {
					from.Get<Poisonable> ().CancelPoison (from);
					RemoveEntityByTag (item.Tag);
				} else {
					var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
					if (from.Get<Consumable<ConsumableTypes>> ().Amount (ConsumableTypes.Antitod) < gameState.PlayerAntitodPotionsMax) {
						from.Get<Consumable<ConsumableTypes>> ().Refill (ConsumableTypes.Antitod, 1);
						RemoveEntityByTag (item.Tag);
						hud.FeedbackMessage ("Picked up antitod potion", Color.Yellow);
					}
				}
			}
		}

		void OnArtefactTriggered (Entity item, Entity from)
		{
			var hud = (HUDLayer)Scene.GetLayer ("hud");
			var mapGridLayer = (MapGridLayer)Scene.GetLayer ("map");

			mapGridLayer.AddExitPoint (true);
			RemoveEntityByTag (item.Tag);

			hud.FeedbackMessage ("Final exit is open", Color.White, 2000);
		}

		void SpawnTrash(Entity spawner)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var map = world.GetLevel(gameState.Level);
			int trash = GetAllEntities ().Where (e => e.Has<State<TreasureTypes>> () && e.Get<State<TreasureTypes>> ().EState == TreasureTypes.Trash).Count();

			if (trash < Globals.MONSTER_POPULATION [gameState.Level]) {
				var tunnels = map.GetTunnels ();
				var pos = tunnels [Utils.ThrowDice (tunnels.Count)];

				var ent = CreateEntity ();
				ent.Register (new Sprite (sprites.GetSprite ("skeleton_01")));
				ent.Register (new Drawable (new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Position (pos.X, pos.Y));
				ent.Register (new Health (Globals.TRASH_HEALTH[gameState.Level], () => OnTrashDestroyed(ent)));
				ent.Register (new Attackable ((attacker, _, __) => OnChestAttacked(ent, attacker)));
				ent.Register (new IFFSystem (Globals.IFF_MONSTER_ID));
				ent.Register (new State<TreasureTypes> (TreasureTypes.Trash));
				ent.Register (new PointLight (PointLight.DistanceType.Small, Color.Gold));
			}
		}

		void SpawnChest(Entity spawner)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var monsters = (MonstersLayer)Scene.GetLayer ("monsters");

			var map = world.GetLevel(gameState.Level);
			var chests = GetAllEntities ().Where (e => e.Has<State<TreasureTypes>> () && e.Get<State<TreasureTypes>> ().EState == TreasureTypes.Chest);

			foreach (var room in map.GetRooms()) {
				if (chests.Any (e => e.Get<Tag> ().ID == room.RoomID))
					continue;

				var pos = new Point (room.Pos.X + Utils.Rand.Next (room.Width), room.Pos.Y + Utils.Rand.Next (room.Height));

				var ent = CreateEntity ();
				ent.Register (new Sprite (sprites.GetSprite ("grave_01")));
				ent.Register (new Drawable (new Vector2(pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));
				ent.Register (new Position (pos.X, pos.Y));
				ent.Register (new Health (Globals.CHEST_HEALTH[gameState.Level], () => OnChestDestroyed(ent)));
				ent.Register (new Attackable ((attacker, _, __) => OnChestAttacked(ent, attacker)));
				ent.Register (new IFFSystem (Globals.IFF_MONSTER_ID));
				ent.Register (new State<TreasureTypes> (TreasureTypes.Chest));
				ent.Register (new PointLight (PointLight.DistanceType.Small, Color.White));
				ent.Register (new Tag (room.RoomID));

				monsters.SpawnBoss (room);
			}
		}
	}
}

