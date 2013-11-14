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

	public class World: IWorld, IGameSerializer
	{
		readonly int WIDTH = 32;
		readonly int HEIGHT = 23;

		IMapGrid[] m_levels;

		public void GenerateLevels(int max_levels)
		{
			m_levels = new IMapGrid[max_levels];

			for (int i = 0; i < max_levels; ++i) {
				var mg = new MapGrid (WIDTH, HEIGHT);
				MapGenerator.Generate (mg, new MapGenerator.Params (mg));
				m_levels[i] = mg;
			}
		}

		public IMapGrid GetLevel (int level)
		{
			return m_levels[level];
		}


		public void Save()
		{
		}

		public void Load()
		{
		}
	}
}

