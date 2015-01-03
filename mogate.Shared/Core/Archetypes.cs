using System;
using System.Collections.Generic;

namespace mogate
{
	public static class Archetypes
	{
		public static readonly Dictionary<string, int>[] Monsters = {
			new Dictionary<string, int> {
				{"spawn_weight", 10},
				{"sprite_index", 1},
				{"health", 100},
				{"attack", 10},
				{"perception", 3},
				{"patrol_min_steps", 3},
				{"patrol_max_steps", 5},
				{"move_duration_msec", 800},
				{"attack_duration_msec", 700},
				{"poison_damage", 0},
				{"poison_chance", 0},
				{"poison_effect_delay_msec", 0},
				{"money_drop", 100},
			},
			new Dictionary<string, int> {
				{"spawn_weight", 20},
				{"sprite_index", 2},
				{"health", 100},
				{"attack", 10},
				{"perception", 3},
				{"patrol_min_steps", 3},
				{"patrol_max_steps", 5},
				{"move_duration_msec", 800},
				{"attack_duration_msec", 700},
				{"poison_damage", 10},
				{"poison_chance", 50},
				{"poison_effect_delay_msec", 1000},
				{"money_drop", 200},
			},
		};

		public static readonly Dictionary<string, int>[] Bosses = {
			new Dictionary<string, int> {
				{"spawn_weight", 20},
				{"sprite_index", 1},
				{"health", 100},
				{"attack", 10},
				{"perception", 3},
				{"patrol_min_steps", 3},
				{"patrol_max_steps", 5},
				{"move_duration_msec", 800},
				{"attack_duration_msec", 700},
				{"poison_damage", 10},
				{"poison_chance", 50},
				{"poison_effect_delay_msec", 1000},
				{"money_drop", 300},
			},
		};
	}
}

