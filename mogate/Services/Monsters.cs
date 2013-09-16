using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace mogate
{
	public enum MonsterState
	{
		Idle,
		Chasing
	};

	public class MonsterData : Entity
	{
		public Point MapPos;
		public Vector2 DrawPos;
		public MonsterState State;

		public MonsterData(int x, int y)
		{
			State = MonsterState.Idle;
			MapPos.X = x;
			MapPos.Y = y;
			DrawPos.X = MapPos.X * 32;
			DrawPos.Y = MapPos.Y * 32;

			RegisterAspect (new Health (100));
		}
	};

	public interface IMonsters
	{
		void Init();
		IEnumerable<MonsterData> GetMonsters();
		void UpdateMapPos (MonsterData monster);
		void UpdateDrawPos (MonsterData data);
	};

	public class Monsters : GameComponent, IMonsters
	{
		List<MonsterData> m_monstersPos = new List<MonsterData>();
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
							m_monstersPos.Add (new MonsterData (x, y));
						}
					}
				}
			}
		}

		public IEnumerable<MonsterData> GetMonsters()
		{
			return m_monstersPos;
		}

		public void UpdateMapPos (MonsterData data)
		{
			if (data.State == MonsterState.Idle) {
				var hero = (IHero)Game.Services.GetService (typeof(IHero));
				var map = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));

				if (MapGenerator.Dist (data.MapPos, hero.Position) > 5)
					return;

				Point newPos = data.MapPos;
				if (hero.Position.X < data.MapPos.X)
					newPos.X--;
				else if (hero.Position.X > data.MapPos.X)
					newPos.X++;
				else if (hero.Position.Y < data.MapPos.Y)
					newPos.Y--;
				else if (hero.Position.Y > data.MapPos.Y)
					newPos.Y++;

				if (newPos == hero.Position)
					return;

				if (m_monstersPos.Any (x => x.MapPos == newPos))
					return;

				if (map.GetID (newPos.X, newPos.Y) == MapGridTypes.ID.Tunnel) {
					data.MapPos = newPos;
					data.State = MonsterState.Chasing;
				}
			}
		}

		public void UpdateDrawPos (MonsterData data)
		{
			if (data.MapPos.X * 32 == data.DrawPos.X && data.MapPos.Y * 32 == data.DrawPos.Y) {
				data.State = MonsterState.Idle;
			} else {
				if (data.DrawPos.X < data.MapPos.X * 32)
					data.DrawPos.X += 4;
				if (data.DrawPos.X > data.MapPos.X * 32)
					data.DrawPos.X -= 4;
				if (data.DrawPos.Y < data.MapPos.Y * 32)
					data.DrawPos.Y += 4;
				if (data.DrawPos.Y > data.MapPos.Y * 32)
					data.DrawPos.Y -= 4;
			}
		}
	}
}

