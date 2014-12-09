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

		public static readonly int HEALTH_PACK = 20;

		public static readonly int PLAYER_HEALTH = 2;
		public static readonly int PLAYER_HEALTH_MAX = 6;
		public static readonly int PLAYER_ATTACK = 10;
		public static readonly int PLAYER_TORCH_MAX = 2;
		public static readonly int PLAYER_MOVE_SPEED = 300;

		public static readonly int TORCH_ATTACK = 10;
		public static readonly int TORCH_HEALTH = 50;

		public static readonly int[] MONSTER_HEALTH = {10, 10, 20, 20, 40};
		public static readonly int[] MONSTER_ATTACK = {20, 20, 30, 40, 50};
		public static readonly int[] MONSTER_MOVE_SPEED = {800, 700, 600, 500, 500};
		public static readonly int[] MONSTER_ATTACK_SPEED = {700, 600, 500, 400, 400};
		public static readonly int[] MONSTER_PERCEPTION = {2, 3, 4, 5, 6};

		public static readonly int BOSS_HEALTH = 100;
		public static readonly int BOSS_ATTACK = 80;
		public static readonly int BOSS_MOVE_SPEED = 400;
		public static readonly int BOSS_ATTACK_SPEED = 300;
		public static readonly int BOSS_PERCEPTION = 10;

		public static readonly int[] DROP_HEALTH_PROB = {30, 30, 30, 30, 30};

		public static readonly int[] MONSTER_PROB = {3, 4, 5, 5, 7};
		public static readonly int[] ROOM_MIN_SIZE = {3, 3, 5, 5, 9};
		public static readonly int[] ROOM_MAX_SIZE = {3, 5, 5, 6, 9};
		public static readonly int[] TUNNELS_CURVE = {5, 3, 4, 6, 5};
		public static readonly bool[] REM_DEAD_END = {false, false, false, false, false};
	}
}

