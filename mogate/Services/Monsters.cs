using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace mogate
{
	public enum MonsterState
	{
		Idle,
		Chasing,
		Dead
	};

	public class MonsterEntity : Entity
	{
		public MonsterState State;

		public MonsterEntity(int x, int y)
		{
			State = MonsterState.Idle;
			Register (new Position (x, y));
			Register (new Health (100));
			Register (new ActionList ());
		}
	};

	public interface IMonsters
	{
		void Init();
		IEnumerable<MonsterEntity> GetMonsters();
		void UpdateMapPos (MonsterEntity monster);
		void UpdateDrawPos (MonsterEntity monster);
		void UpdateActions ();
	};

	public class Monsters : GameComponent, IMonsters
	{
		List<MonsterEntity> m_monsters = new List<MonsterEntity>();
		Random m_rand = new Random(DateTime.UtcNow.Millisecond);


		public Monsters (Game game) : base(game)
		{
		}

		public void Init()
		{
			var map = (IMapGrid)Game.Services.GetService(typeof(IMapGrid));
			m_monsters.Clear ();

			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID (x, y) == MapGridTypes.ID.Tunnel) {
						if (m_rand.Next (100) < 10) {
							m_monsters.Add (new MonsterEntity (x, y));
						}
					}
				}
			}
		}

		public IEnumerable<MonsterEntity> GetMonsters()
		{
			return m_monsters;
		}

		public void UpdateMapPos (MonsterEntity data)
		{
			if (data.State == MonsterState.Idle) {
				var hero = (IHero)Game.Services.GetService (typeof(IHero));
				var map = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));

				if (MapGenerator.Dist (data.Get<Position>().MapPos, hero.Player.Get<Position>().MapPos) > 5)
					return;

				Point newPos = data.Get<Position>().MapPos;
				    if (hero.Player.Get<Position>().MapPos.X < data.Get<Position>().MapPos.X)
					newPos.X--;
				    else if (hero.Player.Get<Position>().MapPos.X > data.Get<Position>().MapPos.X)
					newPos.X++;
				    else if (hero.Player.Get<Position>().MapPos.Y < data.Get<Position>().MapPos.Y)
					newPos.Y--;
				    else if (hero.Player.Get<Position>().MapPos.Y > data.Get<Position>().MapPos.Y)
					newPos.Y++;

				    if (newPos == hero.Player.Get<Position>().MapPos)
					return;

				if (m_monsters.Any (x => x.Get<Position>().MapPos == newPos))
					return;

				if (map.GetID (newPos.X, newPos.Y) == MapGridTypes.ID.Tunnel) {
					data.Get<Position>().MapPos = newPos;
					data.State = MonsterState.Chasing;
				}
			}
		}

		public void UpdateDrawPos (MonsterEntity data)
		{
			if (data.Get<Position>().MapPos.X * 32 == data.Get<Position>().DrawPos.X
			    && data.Get<Position>().MapPos.Y * 32 == data.Get<Position>().DrawPos.Y) {
				data.State = MonsterState.Idle;
			} else {
				if (data.Get<Position>().DrawPos.X < data.Get<Position>().MapPos.X * 32)
					data.Get<Position>().DrawPos.X += 4;
				if (data.Get<Position>().DrawPos.X > data.Get<Position>().MapPos.X * 32)
					data.Get<Position>().DrawPos.X -= 4;
				if (data.Get<Position>().DrawPos.Y < data.Get<Position>().MapPos.Y * 32)
					data.Get<Position>().DrawPos.Y += 4;
				if (data.Get<Position>().DrawPos.Y > data.Get<Position>().MapPos.Y * 32)
					data.Get<Position>().DrawPos.Y -= 4;
			}
		}

		public void UpdateActions ()
		{
			foreach (var me in m_monsters) {
				me.Get<ActionList> ().Update ();
			}
			m_monsters.RemoveAll (x => x.Get<Health> ().HP == 0);
		}
	}
}

