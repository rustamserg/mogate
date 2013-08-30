using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace mogate
{
	public interface IMonsters
	{
		void Init();
		IEnumerable<Point> GetMonsters();
	};

	public class Monsters : GameComponent, IMonsters
	{
		List<Point> m_monstersPos = new List<Point>();
		Random m_rand = new Random(DateTime.UtcNow.Millisecond);


		public Monsters (Game game) : base(game)
		{
		}

		public void Init()
		{
			var map = (IMapGrid)Game.Services.GetService(typeof(IMapGrid));
			m_monstersPos.Clear ();

			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID (x, y) == MapGridTypes.ID.Tunnel) {
						if (m_rand.Next (100) < 10) {
							m_monstersPos.Add (new Point (x, y));
						}
					}
				}
			}
		}

		public override void Update (GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			if (gameState.State == EState.LevelStarted) {

			}

			base.Update (gameTime);
		}

		public IEnumerable<Point> GetMonsters()
		{
			return new List<Point> (m_monstersPos);
		}
	}
}

