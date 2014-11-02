using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace mogate
{
	public interface IWorld
	{
		void GenerateLevels(int max_levels);
		IMapGrid GetLevel (int level);
	}

	public class World: IWorld
	{
		IMapGrid[] m_levels;

		public void GenerateLevels(int max_levels)
		{
			m_levels = new IMapGrid[max_levels];

			for (int i = 0; i < max_levels; ++i) {
				var mg = new MapGrid (Globals.WORLD_WIDTH, Globals.WORLD_HEIGHT);
				var pr = new MapGenerator.Params (mg);

				pr.RoomMinSize = Globals.ROOM_MIN_SIZE [i];
				pr.RoomMaxSize = Globals.ROOM_MAX_SIZE [i];
				pr.RemoveDeadEnd = Globals.REM_DEAD_END [i];
				pr.TunnelsCurveWeight = Globals.TUNNELS_CURVE [i];

				MapGenerator.Generate (mg, pr);
				m_levels[i] = mg;
			}
		}

		public IMapGrid GetLevel (int level)
		{
			return m_levels[level];
		}
	}
}

