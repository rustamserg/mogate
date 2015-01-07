using System;

namespace mogate
{
	public static class Globals
	{
		public static readonly int VIEWPORT_WIDTH = 1024;
		public static readonly int VIEWPORT_HEIGHT = 768;

		public static readonly int WORLD_WIDTH = 31;
		public static readonly int WORLD_HEIGHT = 23;

		public static readonly int CELL_WIDTH = 32;
		public static readonly int CELL_HEIGHT = 32;

		public static readonly int MAX_LEVELS = 5;

		public static readonly int IFF_PLAYER_ID = 0;
		public static readonly int IFF_MONSTER_ID = 1;

		public static readonly int HEALTH_PACK = 40;

		public static readonly int HALL_OF_FAME_SIZE = 5;

		public static readonly int PLAYER_HEALTH = 3;
		public static readonly int PLAYER_HEALTH_MAX = 6;
		public static readonly int PLAYER_MOVE_SPEED = 300;
		public static readonly int PLAYER_SPRITES_MAX = 4;

		public static readonly int[] MONSTER_DROP_LOOT_WEIGHT = {30, 30, 30, 30, 30};
		public static readonly int[] CHEST_DROP_LOOT_WEIGHT = {10, 20, 30, 40, 40};

		public static readonly int[] MONSTER_POPULATION = {8, 10, 12, 15, 20};
		public static readonly int[] MONSTER_SPAWN_DELAY_MSEC = {400, 300, 200, 100, 50};
		public static readonly int[] MONSTER_SPAWN_WEIGHT = {10, 20, 30, 30, 40};
		public static readonly int[] BOSSES_SPAWN_WEIGHT = {10, 20, 30, 30, 40};

		public static readonly int[] MAP_WALLS_MAX = {3, 3, 1, 3, 1, 3};
		public static readonly int[] MAP_TILES_ID = {1, 1, 2, 1, 2, 1};
		public static readonly int[] ROOM_MIN_SIZE = {3, 3, 5, 5, 9};
		public static readonly int[] ROOM_MAX_SIZE = {3, 5, 5, 6, 9};
		public static readonly int[] TUNNELS_CURVE = {5, 3, 4, 6, 5};
		public static readonly bool[] REM_DEAD_END = {false, false, false, false, false};
	}
}

