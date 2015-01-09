﻿using System;
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
				{ "critical_chance", 10 },
				{ "critical_damage", 20 },
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
				{ "critical_chance", 10 },
				{ "critical_damage", 20 },
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
				{ "critical_chance", 50 },
				{ "critical_damage", 50 },
			},
		};

		public static readonly Dictionary<string, int>[] Bosses = {
			new Dictionary<string, int> {
				{ "spawn_weight", 10 },
				{ "sprite_index", 1 },
				{ "health", 1000 },
				{ "attack", 20 },
				{ "perception", 3 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 900 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 10 },
				{ "poison_chance", 0 },
				{ "poison_effect_delay_msec", 600 },
				{ "visible", 0 },
				{ "critical_chance", 10 },
				{ "critical_damage", 20 },
			},
			new Dictionary<string, int> {
				{ "spawn_weight", 20 },
				{ "sprite_index", 2 },
				{ "health", 1000 },
				{ "attack", 50 },
				{ "perception", 3 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 800 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 10 },
				{ "poison_chance", 50 },
				{ "poison_effect_delay_msec", 600 },
				{ "visible", 0 },
				{ "critical_chance", 20 },
				{ "critical_damage", 20 },
			},
			new Dictionary<string, int> {
				{ "spawn_weight", 30 },
				{ "sprite_index", 3 },
				{ "health", 1000 },
				{ "attack", 50 },
				{ "perception", 3 },
				{ "patrol_min_steps", 3 },
				{ "patrol_max_steps", 5 },
				{ "move_duration_msec", 700 },
				{ "attack_duration_msec", 700 },
				{ "poison_damage", 10 },
				{ "poison_chance", 50 },
				{ "poison_effect_delay_msec", 600 },
				{ "visible", 0 },
				{ "critical_chance", 40 },
				{ "critical_damage", 20 },
			},
			new Dictionary<string, int> {
				{ "spawn_weight", 40 },
				{ "sprite_index", 4 },
				{ "health", 2000 },
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
				{ "critical_chance", 60 },
				{ "critical_damage", 20 },
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
				{ "critical_chance", 0 },
				{ "critical_damage", 0 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 2 },
				{ "attack", 50 },
				{ "critical_chance", 10 },
				{ "critical_damage", 20 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 3 },
				{ "attack", 100 },
				{ "critical_chance", 50 },
				{ "critical_damage", 20 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 4 },
				{ "attack", 200 },
				{ "critical_chance", 60 },
				{ "critical_damage", 20 },
			},
		};

		public static readonly Dictionary<string, int>[] MonsterLoot = {
			new Dictionary<string, int> {
				{"spawn_weight", 30},
				{"loot_type", (int)LootTypes.Money},
				{"lifetime_sec", 10},
				{"money", Globals.MONEY_PACK},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 15},
				{"loot_type", (int)LootTypes.Health},
				{"lifetime_sec", 10},
				{"health", Globals.HEALTH_PACK},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 7},
				{"loot_type", (int)LootTypes.Antitod},
				{"lifetime_sec", 10},
			},
		};

		public static readonly Dictionary<string, int>[] TrashLoot = {
			new Dictionary<string, int> {
				{"spawn_weight", 30},
				{"loot_type", (int)LootTypes.Money},
				{"lifetime_sec", 10},
				{"money", (int)Globals.MONEY_PACK/5},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 15},
				{"loot_type", (int)LootTypes.Health},
				{"lifetime_sec", 10},
				{"health", (int)Globals.HEALTH_PACK/4},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 5},
				{"loot_type", (int)LootTypes.Antitod},
				{"lifetime_sec", 10},
			},
		};

		public static readonly Dictionary<string, int>[] ChestLoot = {
			new Dictionary<string, int> {
				{"spawn_weight", 10},
				{"loot_type", (int)LootTypes.Money},
				{"lifetime_sec", 10},
				{"money", Globals.MONEY_PACK*2},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 10},
				{"loot_type", (int)LootTypes.Armor},
				{"lifetime_sec", 10},
				{"armor_index", 0},
				{"price", Globals.MONEY_PACK*4},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 20},
				{"loot_type", (int)LootTypes.Armor},
				{"lifetime_sec", 10},
				{"armor_index", 1},
				{"price", Globals.MONEY_PACK*4},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 30},
				{"loot_type", (int)LootTypes.Armor},
				{"lifetime_sec", 10},
				{"armor_index", 2},
				{"price", Globals.MONEY_PACK*5},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 40},
				{"loot_type", (int)LootTypes.Armor},
				{"lifetime_sec", 10},
				{"armor_index", 3},
				{"price", Globals.MONEY_PACK*6},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 20},
				{"loot_type", (int)LootTypes.Weapon},
				{"lifetime_sec", 10},
				{"weapon_index", 1},
				{"price", Globals.MONEY_PACK*4},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 30},
				{"loot_type", (int)LootTypes.Weapon},
				{"lifetime_sec", 10},
				{"weapon_index", 2},
				{"price", Globals.MONEY_PACK*5},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 40},
				{"loot_type", (int)LootTypes.Weapon},
				{"lifetime_sec", 10},
				{"weapon_index", 3},
				{"price", Globals.MONEY_PACK*6},
			},
		};

		public static readonly Dictionary<string, int>[] Players = {
			new Dictionary<string, int> {
				{ "sprite_index", 1 },
				{ "health_packs", 3 },
				{ "health_packs_max", 6 },
				{ "move_duration_msec", 300 },
				{ "money_multiplier", 1 },
				{ "attack_multiplier", 2 },
				{ "poison_multiplier", 1 },
				{ "view_distance_type", 1 },
				{ "attack_distance", 1 },
				{ "attack_duration_msec", 200 },
				{ "antitod_potions_max", 3 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 2 },
				{ "health_packs", 5 },
				{ "health_packs_max", 8 },
				{ "move_duration_msec", 400 },
				{ "money_multiplier", 3 },
				{ "attack_multiplier", 1 },
				{ "poison_multiplier", 1 },
				{ "view_distance_type", 1 },
				{ "attack_distance", 1 },
				{ "attack_duration_msec", 200 },
				{ "antitod_potions_max", 3 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 3 },
				{ "health_packs", 2 },
				{ "health_packs_max", 4 },
				{ "move_duration_msec", 200 },
				{ "money_multiplier", 1 },
				{ "attack_multiplier", 1 },
				{ "poison_multiplier", 1 },
				{ "view_distance_type", 2 },
				{ "attack_distance", 3 },
				{ "attack_duration_msec", 100 },
				{ "antitod_potions_max", 3 },
			},
			new Dictionary<string, int> {
				{ "sprite_index", 4 },
				{ "health_packs", 8 },
				{ "health_packs_max", 12 },
				{ "move_duration_msec", 500 },
				{ "money_multiplier", 1 },
				{ "attack_multiplier", 1 },
				{ "poison_multiplier", 0 },
				{ "view_distance_type", 1 },
				{ "attack_distance", 1 },
				{ "attack_duration_msec", 400 },
				{ "antitod_potions_max", 3 },
			},
		};
	}
}