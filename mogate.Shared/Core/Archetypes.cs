using System;
using System.Collections.Generic;

namespace mogate
{
	public static class Archetypes
	{
		public static readonly Dictionary<string, int>[] Monsters = {
			new Dictionary<string, int> {
				{ "spawn_weight", 10 },
				{ "sprite_index", 1 },
				{ "health", 100 },
				{ "attack", 10 },
				{ "perception", 3 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 800 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 0 },
				{ "poison_chance", 0 },
				{ "poison_effect_delay_msec", 0 },
				{ "visible", 1 },
				{ "critical_chance", 0 },
				{ "critical_damage", 0 },
			},
			new Dictionary<string, int> {
				{ "spawn_weight", 20 },
				{ "sprite_index", 2 },
				{ "health", 200 },
				{ "attack", 20 },
				{ "perception", 5 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 800 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 10 },
				{ "poison_chance", 50 },
				{ "poison_effect_delay_msec", 1000 },
				{ "visible", 1 },
				{ "critical_chance", 0 },
				{ "critical_damage", 0 },
			},
			new Dictionary<string, int> {
				{ "spawn_weight", 30 },
				{ "sprite_index", 3 },
				{ "health", 300 },
				{ "attack", 30 },
				{ "perception", 3 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 300 },
				{ "attack_duration_msec", 500 },
				{ "poison_damage", 20 },
				{ "poison_chance", 70 },
				{ "poison_effect_delay_msec", 1000 },
				{ "visible", 0 },
				{ "critical_chance", 0 },
				{ "critical_damage", 0 },
			},
			new Dictionary<string, int> {
				{ "spawn_weight", 40 },
				{ "sprite_index", 4 },
				{ "health", 500 },
				{ "attack", 100 },
				{ "perception", 3 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 800 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 5 },
				{ "poison_chance", 20 },
				{ "poison_effect_delay_msec", 500 },
				{ "visible", 0 },
				{ "critical_chance", 0 },
				{ "critical_damage", 0 },
			},
		};

		public static readonly Dictionary<string, int>[] Bosses = {
			new Dictionary<string, int> {
				{ "spawn_weight", 10 },
				{ "sprite_index", 1 },
				{ "health", 2000 },
				{ "attack", 100 },
				{ "perception", 3 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 900 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 10 },
				{ "poison_chance", 0 },
				{ "poison_effect_delay_msec", 600 },
				{ "visible", 0 },
				{ "critical_chance", 0 },
				{ "critical_damage", 0 },
			},
			new Dictionary<string, int> {
				{ "spawn_weight", 20 },
				{ "sprite_index", 2 },
				{ "health", 3000 },
				{ "attack", 100 },
				{ "perception", 3 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 800 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 10 },
				{ "poison_chance", 50 },
				{ "poison_effect_delay_msec", 600 },
				{ "visible", 0 },
				{ "critical_chance", 0 },
				{ "critical_damage", 0 },
			},
			new Dictionary<string, int> {
				{ "spawn_weight", 30 },
				{ "sprite_index", 3 },
				{ "health", 4000 },
				{ "attack", 100 },
				{ "perception", 3 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 700 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 10 },
				{ "poison_chance", 50 },
				{ "poison_effect_delay_msec", 600 },
				{ "visible", 0 },
				{ "critical_chance", 0 },
				{ "critical_damage", 0 },
			},
			new Dictionary<string, int> {
				{ "spawn_weight", 40 },
				{ "sprite_index", 4 },
				{ "health", 5000 },
				{ "attack", 100 },
				{ "perception", 5 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 600 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 10 },
				{ "poison_chance", 50 },
				{ "poison_effect_delay_msec", 600 },
				{ "visible", 0 },
				{ "critical_chance", 0 },
				{ "critical_damage", 0 },
			},
		};

		public static readonly Dictionary<string, int>[] Armors = {
			new Dictionary<string, int> {
				{ "sprite_index", 1 },
				{ "defence", 5 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 2 },
				{ "defence", 10 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 3 },
				{ "defence", 20 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 4 },
				{ "defence", 50 },
			},
		};

		public static readonly Dictionary<string, int>[] Weapons = {
			new Dictionary<string, int> {
				{ "sprite_index", 1 },
				{ "attack", 20 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 2 },
				{ "attack", 50 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 3 },
				{ "attack", 100 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 4 },
				{ "attack", 200 },
			},
		};

		public static readonly Dictionary<string, int>[] MonsterLoot = {
			new Dictionary<string, int> {
				{"spawn_weight", 30},
				{"loot_type", (int)LootTypes.Money},
				{"money", 100},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 15},
				{"loot_type", (int)LootTypes.Health},
				{"health", Globals.HEALTH_PACK},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 7},
				{"loot_type", (int)LootTypes.Antitod},
			},
		};

		public static readonly Dictionary<string, int>[] ChestLoot = {
			new Dictionary<string, int> {
				{"spawn_weight", 10},
				{"loot_type", (int)LootTypes.Armor},
				{"armor_index", 0},
				{"price", 200},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 20},
				{"loot_type", (int)LootTypes.Armor},
				{"armor_index", 1},
				{"price", 300},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 30},
				{"loot_type", (int)LootTypes.Armor},
				{"armor_index", 2},
				{"price", 400},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 40},
				{"loot_type", (int)LootTypes.Armor},
				{"armor_index", 3},
				{"price", 500},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 10},
				{"loot_type", (int)LootTypes.Weapon},
				{"weapon_index", 0},
				{"price", 200},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 20},
				{"loot_type", (int)LootTypes.Weapon},
				{"weapon_index", 1},
				{"price", 300},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 30},
				{"loot_type", (int)LootTypes.Weapon},
				{"weapon_index", 2},
				{"price", 400},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 40},
				{"loot_type", (int)LootTypes.Weapon},
				{"weapon_index", 3},
				{"price", 500},
			},
		};
	}
}