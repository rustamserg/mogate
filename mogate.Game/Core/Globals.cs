using System;

namespace mogate
{
	public static class Globals
	{
		public static readonly int WORLD_WIDTH = 31;
		public static readonly int WORLD_HEIGHT = 23;

		public static readonly int CELL_WIDTH = 32;
		public static readonly int CELL_HEIGHT = 32;

		public static readonly int MAX_LEVELS = 5;

		public static readonly int IFF_PLAYER_ID = 0;
		public static readonly int IFF_MONSTER_ID = 1;

		public static readonly int[] MONSTER_PROB = {5, 8, 10, 15, 20};
		public static readonly int[] ROOM_MIN_SIZE = {3, 3, 5, 5, 9};
		public static readonly int[] ROOM_MAX_SIZE = {3, 5, 5, 6, 9};
		public static readonly int[] TUNNELS_CURVE = {5, 3, 4, 6, 5};
		public static readonly bool[] REM_DEAD_END = {false, false, false, false, false};
	}
}

